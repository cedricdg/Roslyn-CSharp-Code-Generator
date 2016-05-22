using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpCodeGenerator;
using CSharpCodeGenerator.DataStructures;
using Entitas.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace Entitas.CodeGenerator
{
    public class RoslynComponentInfoProvider : ICodeGeneratorDataProvider
    {
        private const string POOL_ATTRIBUTE_IDENTIFIER = "Pool";
        string[] _poolNames;
        string[] _blueprintNames;
        ComponentInfo[] _componentInfos;
        private MSBuildWorkspace _workspace;
        private Compilation _compilation;
        private Project _project;

        public string[] blueprintNames
        {
            get
            {
                return _blueprintNames;
            }
        }

        public ComponentInfo[] componentInfos
        {
            get
            {
                return _componentInfos;
            }
        }

        public string[] poolNames
        {
            get
            {
                return _poolNames;
            }
        }


        public RoslynComponentInfoProvider(Project project, string[] poolNames, string[] blueprintNames)
        {
            _poolNames = poolNames;
            _blueprintNames = blueprintNames;

            if (project != null)
            {
                _workspace = MSBuildWorkspace.Create();
                _project = project;

                var compTask = project.GetCompilationAsync();
                _compilation = compTask.Result;
                var diagnostics = _compilation.GetDiagnostics();
                foreach (var d in diagnostics)
                {
//                    Console.WriteLine(d.GetMessage());
                }

                _componentInfos = GetComponentInfos();
            }
        }

        private ComponentInfo[] GetComponentInfos()
        {
            // get all syntax nodes where a class is defined
            // get all information from class
            var componentInfos = new List<ComponentInfo>();
            foreach (var syntaxTree in _compilation.SyntaxTrees)
            {
                var document = new DocumentStructure(syntaxTree.GetCompilationUnitRoot());
                Console.WriteLine(document.Classes.Length);
                foreach (var classStructure in document.Classes)
                {
                    if (IsValidIComponentClass(classStructure))
                    {
                        componentInfos.Add(GetComponentInfo(classStructure));
                    }
                }
            }
            return componentInfos.ToArray();
        }

        private bool IsValidIComponentClass(ClassStructure classNode)
        {
            if (classNode.ModifierFlags.HasFlag(ModifierFlags.Abstract))
                return false;
            return true;
        }

        private ComponentInfo GetComponentInfo(ClassStructure classDeclarationNode)
        {
            var pools = GetPools(classDeclarationNode.Attributes);
            var fullClassName = classDeclarationNode.FullClassName;
            List<PublicMemberInfo> publicMemberInfos = GetPublicMemberInfos(classDeclarationNode.Fields);
            return new ComponentInfo(fullClassName, publicMemberInfos, pools,
                isSingleEntity: false, generateMethods: true, generateComponent: true, generateIndex: true, singleComponentPrefix: "is");
        }

        private List<PublicMemberInfo> GetPublicMemberInfos(FieldStructure[] fields)
        {
            var result = new List<PublicMemberInfo>();
            foreach (var field in fields)
            {
//                new PublicMemberInfo();
            }
            return result;
        }

        private static string[] GetPools(AttributeStructure[] attributes)
        {
            return attributes.Where(a => a.Identifier.Equals(POOL_ATTRIBUTE_IDENTIFIER)).Select(GetPoolValueWithoutQuotes).ToArray();
        }

        private static string GetPoolValueWithoutQuotes(AttributeStructure attribute)
        {
            return attribute.Values.First().Replace("\"", string.Empty);
        }
    }
}

