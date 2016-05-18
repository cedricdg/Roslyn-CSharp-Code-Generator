using System;
using System.Collections.Generic;
using System.Linq;
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
                _componentInfos = GetComponentInfos();
            }
        }

        private ComponentInfo[] GetComponentInfos()
        {
            // get all syntax nodes where a class is defined
            // get all information from class
            var diagnostics = _compilation.GetDiagnostics();
            foreach (var d in diagnostics)
            {
                Console.WriteLine(d.GetMessage());
            }
            var componentInfos = new List<ComponentInfo>();
            foreach (var syntaxTree in _compilation.SyntaxTrees)
            {

                var semanticModel = _compilation.GetSemanticModel(syntaxTree);
                componentInfos.Add(GetComponentInfoForSyntaxTree(semanticModel));
            }
            return componentInfos.ToArray();
        }

        private ComponentInfo GetComponentInfoForSyntaxTree(SemanticModel semanticModel)
        {
            var document = new DocumentStructure(semanticModel.SyntaxTree.GetCompilationUnitRoot());

            var classDeclarationNode = new ClassStructure(document.Classes.First());

            var info = semanticModel.GetSymbolInfo(classDeclarationNode.Node);
            var symbol = info.Symbol;
            
            return new ComponentInfo(classDeclarationNode.Node.Identifier.Text, new List<PublicMemberInfo>(), new string[0],
                isSingleEntity: false, generateMethods: true, generateComponent: true, generateIndex: true, singleComponentPrefix: "is");
        }
    }
}

