using System.Collections.Generic;
using System.Linq;
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
        private Compilation _compilation;

        public ProjectStructure(Project project)
        {
            Project = project;
        }

        public IEnumerable<DocumentStructure> Documents {
            get
            {
                foreach (var d in Project.Documents)
                    yield return new DocumentStructure(d.GetSyntaxRootAsync().Result.SyntaxTree.GetCompilationUnitRoot());
            }
        }

        public static ProjectStructure Load(string path)
        {
            var workspace = MSBuildWorkspace.Create();
            var fixturesProject = workspace.OpenProjectAsync(path).Result;

            return new ProjectStructure(fixturesProject);
        }

        public string GetFullMetadataName(FieldStructure field)
        {
            if (_compilation == null)
            {
                var compTask = Project.GetCompilationAsync();
                _compilation = compTask.Result;
            }

            var semanticModel = _compilation.GetSemanticModel(_compilation.SyntaxTrees.First());
            var typeInfo = semanticModel.GetSpeculativeTypeInfo(field.Node.SpanStart, field.Node.Declaration.Type, SpeculativeBindingOption.BindAsTypeOrNamespace);
            var typeSymbol = typeInfo.Type;

            var symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
            string fullyQualifiedName = typeSymbol.ToDisplayString(symbolDisplayFormat);
            return fullyQualifiedName;
        }
    }
}
