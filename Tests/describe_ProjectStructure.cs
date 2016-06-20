
using System;
using System.IO;
using System.Linq;
using CSharpCodeGenerator.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSpec;

namespace CSharpCodeGenerator.Tests.DataStructures
{
    class describe_ProjectStructure : nspec
    {
        void when_wrapping()
        {
            it["loads project"] = () =>
            {
                var projectPath = "../../CSharpCodeGenerator.Tests.csproj";
                var project = ProjectStructure.Load(projectPath);
                project.Project.FilePath.should_be(Path.GetFullPath(projectPath));
            };

            it["returns all documents of project"] = () =>
            {
                var workspace = new AdhocWorkspace();
                var projectMock = workspace.AddProject("TestProject", LanguageNames.CSharp);
                var document = projectMock.AddDocument("doc", "class A{}");
                document = document.Project.AddDocument("doc2", "class B{}");
                var project = new ProjectStructure(document.Project);
                project.Documents.Count().should_be(2);
            };



            it["returns full string type form compilation"] = () =>
            {
                var identifier = "testIdentifier";
                var tree = CSharpSyntaxTree.ParseText($"class Testclass {{ public string {identifier} = 1; }}");
                var field = new FieldStructure(tree.GetRoot().DescendantNodes().Single(n => n.IsKind(SyntaxKind.FieldDeclaration)) as FieldDeclarationSyntax);

                var workspace = new AdhocWorkspace();
                var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
                var stringlib = MetadataReference.CreateFromFile(typeof(string).Assembly.Location);
                var project = workspace.AddProject("Test", LanguageNames.CSharp)
                                            .AddMetadataReference(mscorlib)
                                            .AddMetadataReference(stringlib)
                                            .AddDocument("Document", tree.GetRoot()).Project;
                var projectStructure = new ProjectStructure(project);

                projectStructure.GetFullMetadataName(field).should_be(typeof(string).FullName);
            };
        }
    }
}
