using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpCodeGenerator.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CSharpCodeGenerator
{
    public class ProjectStructure : DataStructure
    {
        public readonly Project Project;
        private Compilation _compilation ;

        internal ProjectStructure(Project project)
        {
            Project = project;
        }

        public IEnumerable<DocumentStructure> Documents {
            get
            {
                return Project.Documents
                    .Select(d => new DocumentStructure(d.GetSyntaxRootAsync().Result.SyntaxTree.GetCompilationUnitRoot()));
            }
        }

        public static ProjectStructure Load(string path)
        {
            var workspace = MSBuildWorkspace.Create();
            var fixturesProject = workspace.OpenProjectAsync(path);
            var project = fixturesProject.Result;

            return new ProjectStructure(project);
        }

        public string GetFullTypeName(TypeStructure typeStructure)
        {
            if (_compilation == null)
            {
                _compilation = Project.GetCompilationAsync().Result;
            }

            var typeNode = typeStructure.Node;
            var semanticModel = _compilation.GetSemanticModel(typeNode.SyntaxTree);
            var typeInfo = semanticModel.GetSpeculativeTypeInfo(typeNode.SpanStart, typeNode, SpeculativeBindingOption.BindAsTypeOrNamespace);

            var symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
            var fullyQualifiedName = typeInfo.Type.ToDisplayString(symbolDisplayFormat);

            return fullyQualifiedName;
        }
    }
}
