
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CSharpCodeGenerator.DataStructures;
using CSharpCodeGenerator.Writer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSpec;

namespace CSharpCodeGenerator.Tests.DataStructures
{
    class describe_ProjectWriter : nspec
    {
        void when_writing()
        {
            it["deploys files to folder"] = () =>
            {
                var workspace = new AdhocWorkspace();
                var project = workspace.AddProject("TestProject", LanguageNames.CSharp);

                var expectedFolder = "TestFolder";
                var expectedName = "Doc2";
                var expectedContent = "Content";

                var writer = new ProjectWriter(project, expectedFolder);
                writer.AddFile(new SourceCodeFile { Content = expectedContent, Name = expectedName });
                writer.Project.Documents.Single().Name.should_be(expectedName);
                writer.Project.Documents.Single().GetTextAsync().Result.ToString().should_be(expectedContent);
                writer.Project.Documents.Single().Folders.Single().should_be(expectedFolder);
            };

            it["removes folder and contents"] = () =>
            {
                var workspace = new AdhocWorkspace();
                var project = workspace.AddProject("TestProject", LanguageNames.CSharp);

                var targetFolderPath = "TestFolder";
                var expectedName1 = "ShouldSurviveDocument1";
                var expectedName2 = "ShouldSurviveDocument2";
                // dont remove
                project = project.AddDocument(expectedName1, string.Empty, new[] { "Folder1", targetFolderPath }).Project;
                project = project.AddDocument(expectedName2, string.Empty).Project;
                // remove
                project = project.AddDocument("InTargetFolderRoot", string.Empty, new[] { targetFolderPath }).Project;
                project = project.AddDocument("InTargetSubfolder", string.Empty, new[] { targetFolderPath, "test" }).Project;

                var writer = new ProjectWriter(project, targetFolderPath);
                writer.ClearTargetFolder();
                writer.Project.Documents.Count().should_be(2);
                writer.Project.Documents.Any(d => d.Name == expectedName1).should_be_true();
                writer.Project.Documents.Any(d => d.Name == expectedName2).should_be_true();
            };
        }
    }
}
