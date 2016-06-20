using System;
using System.Collections.Generic;
using System.Linq;
using CSharpCodeGenerator;
using CSharpCodeGenerator.DataStructures;
using Entitas.Serialization;

namespace Entitas.CodeGenerator
{
    public class RoslynComponentInfoProvider : ICodeGeneratorDataProvider
    {
        private const string POOL_ATTRIBUTE_IDENTIFIER = "Pool";
        string[] _poolNames;
        string[] _blueprintNames;
        ComponentInfo[] _componentInfos;
        private ProjectStructure _project;

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


        public RoslynComponentInfoProvider(ProjectStructure project, string[] poolNames, string[] blueprintNames)
        {
            _poolNames = poolNames;
            _blueprintNames = blueprintNames;

            if (project != null)
            {
                _project = project;
                

                _componentInfos = GetComponentInfos();
            }
        }

        private ComponentInfo[] GetComponentInfos()
        {
            // get all syntax nodes where a class is defined
            // get all information from class
            var componentInfos = new List<ComponentInfo>();
            foreach (var document in _project.Documents)
            {
                Console.WriteLine(document.Classes.Count());
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
            var isSingleEntity = false;
            return new ComponentInfo(fullClassName, publicMemberInfos, pools.ToArray(),
                isSingleEntity: isSingleEntity, generateMethods: true, generateComponent: true, generateIndex: true, singleComponentPrefix: "is");
        }

        private List<PublicMemberInfo> GetPublicMemberInfos(IEnumerable<FieldStructure> fields)
        {
            var result = new List<PublicMemberInfo>();
            foreach (var field in fields)
            {
                if (field.AccessModifier == AccessModifier.Public && field.ModifierFlags.Equals(ModifierFlags.None))
                {
                    result.Add(new PublicMemberInfo(_project.GetFullMetadataName(field), field.Identifier));
                }
            }
            return result;
        }

        private static IEnumerable<string> GetPools(IEnumerable<AttributeStructure> attributes)
        {
            return attributes.Where(a => a.Identifier.Equals(POOL_ATTRIBUTE_IDENTIFIER)).Select(GetPoolValueWithoutQuotes);
        }

        private static string GetPoolValueWithoutQuotes(AttributeStructure attribute)
        {
            return attribute.Values.First().Replace("\"", string.Empty);
        }
    }
}

