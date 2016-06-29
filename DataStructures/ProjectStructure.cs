using System.Collections.Generic;
using System.Linq;
using CSharpCodeGenerator.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CSharpCodeGenerator
{
    public class ProjectStructure
    {
        internal readonly Project Project;
        private Compilation _compilation;

        internal Compilation Compilation => _compilation ?? (_compilation = Project.GetCompilationAsync().Result);

        internal ProjectStructure(Project project)
        {
            Project = project;
        }

        public static ProjectStructure Load(string path)
        {
            var workspace = MSBuildWorkspace.Create();
            var projectTask = workspace.OpenProjectAsync(path);
            var project = projectTask.Result;

            return new ProjectStructure(project);
        }

        public IEnumerable<DocumentStructure> Documents 
            => from d in Project.Documents
                select new DocumentStructure(d.GetSyntaxRootAsync().Result.SyntaxTree.GetCompilationUnitRoot());

        public IEnumerable<ClassStructure> FindClassesWithImplementedInterface(string interfaceName)
        {
            return Compilation
                .SyntaxTrees.Select(syntaxTree => Compilation.GetSemanticModel(syntaxTree))
                .SelectMany(semanticModel => semanticModel
                        .SyntaxTree
                        .GetRoot()
                        .DescendantNodes()
                        .OfType<ClassDeclarationSyntax>()
                            .Where(classSyntax => semanticModel.GetDeclaredSymbol(classSyntax).Interfaces
                                            .Any(interfaceSymbol => interfaceSymbol.ToDisplayString() == interfaceName)))
                .Select(classNode => new ClassStructure(classNode));
        }

        public string GetFullTypeName(TypeStructure typeStructure)
        {
            var typeNode = typeStructure.Node;
            var semanticModel = Compilation.GetSemanticModel(typeNode.SyntaxTree);

            var typeInfo = semanticModel.GetSpeculativeTypeInfo(typeNode.SpanStart, typeNode, SpeculativeBindingOption.BindAsTypeOrNamespace);

            var symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
            var fullyQualifiedName = typeInfo.Type.ToDisplayString(symbolDisplayFormat);

            return fullyQualifiedName;
        }
    }
}
