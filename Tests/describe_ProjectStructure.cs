
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CSharpCodeGenerator.DataStructures;
using Entitas;
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



            it["returns full string type from compilation"] = () =>
            {
                var identifier1 = "testIdentifier1";
                var identifier2 = "testIdentifier2";
                var type1 = typeof(string);
                var type2 = typeof(object);

                var tree1 = CSharpSyntaxTree.ParseText($"class Testclass1 {{ public {type1} {identifier1} = 1; }}");
                var tree2 = CSharpSyntaxTree.ParseText($"class Testclass2 {{ public {type2} {identifier2} = 1; }}");

                var workspace = new AdhocWorkspace();
                var mscorlib = MetadataReference.CreateFromFile(type1.Assembly.Location);
                var stringlib = MetadataReference.CreateFromFile(type2.Assembly.Location);
                var project = workspace.AddProject("Test", LanguageNames.CSharp)
                                            .AddMetadataReference(mscorlib)
                                            .AddMetadataReference(stringlib)
                                            .AddDocument("Document1", tree1.GetRoot()).Project
                                            .AddDocument("Document2", tree2.GetRoot()).Project;

                var projectStructure = new ProjectStructure(project);

                var field1 = projectStructure.Documents.ElementAt(0).Classes.First().Fields.First();
                var fullMetadataName1 = projectStructure.GetFullTypeName(field1.DeclarationType);

                var field2 = projectStructure.Documents.ElementAt(1).Classes.First().Fields.First();
                var fullMetadataName2 = projectStructure.GetFullTypeName(field2.DeclarationType);


                fullMetadataName1.should_be(type1.FullName);
                fullMetadataName2.should_be(type2.FullName);

            };


            it["returns all classes implementing interface"] = () =>
            {
                var identifier1 = "testIdentifier1";
                var identifier2 = "testIdentifier2";
                var type1 = typeof(string);
                var type2 = typeof(IComponent);

                var tree1 = CSharpSyntaxTree.ParseText($"class Testclass1 : Entitas.IComponent {{ public {type1} {identifier1} = 1; }}");
                var tree2 = CSharpSyntaxTree.ParseText($"class Testclass2 : Entitas.IComponendt {{ public {type2} {identifier2} = 1; }}");

                var workspace = new AdhocWorkspace();
                var mscorlib = MetadataReference.CreateFromFile(type1.Assembly.Location);
                var stringlib = MetadataReference.CreateFromFile(type2.Assembly.Location);
                var project = workspace.AddProject("Test", LanguageNames.CSharp)
                                            .AddMetadataReference(mscorlib)
                                            .AddMetadataReference(stringlib)
                                            .AddDocument("Document1", tree1.GetRoot()).Project
                                            .AddDocument("Document2", tree2.GetRoot()).Project;

                var projectStructure = new ProjectStructure(project);

                var classes = projectStructure.FindClassesWithImplementedInterface("Entitas.IComponent");
                classes.Count().should_be(1);
            };
        }
    }
}
