using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpCodeGenerator.Extensions;
using CSharpCodeGenerator.Tests.Fixtures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using NSpec;
using NSpec.Domain.Extensions;

namespace CSharpCodeGenerator.Tests
{
    class describe_CompilationExtensions : nspec
    {

        void when_executing()
        {
            context["GetTypeDeclarationAsSyntaxNode"] = () => {

                string projectPath = @"..\..\CSharpCodeGenerator.Tests.csproj";
                var workspace = MSBuildWorkspace.Create();

                var project = workspace.OpenProjectAsync(projectPath).Result;
                var compilation = project.GetCompilationAsync().Result;

                it["should return declaration source code from type"] = () =>
                {
                    var definition = CompilationHelper.GetTypeDeclarationAsSyntaxNode(compilation, typeof(PublicTestClass).FullName);
                    var expectedTree = CSharpSyntaxTree.ParseText("public class PublicTestClass{public bool publicTestProperty;}");

                    NormalizeAndAssertNodes(definition, expectedTree.GetRoot());
                };

                it["should only return declaration of type from file with multiple classes"] = () =>
                {
                    var definition = CompilationHelper.GetTypeDeclarationAsSyntaxNode(compilation, typeof(MultipleClassesClass1).FullName);
                    var expectedTree = CSharpSyntaxTree.ParseText("public class MultipleClassesClass1{}");

                    NormalizeAndAssertNodes(definition, expectedTree.GetRoot());
                };

            };
        }

        private void NormalizeAndAssertNodes(SyntaxNode nodeA, SyntaxNode nodeB)
        {
            nodeA.NormalizeWhitespace().ToString().should_be(nodeB.NormalizeWhitespace().ToString());
        }
    }
}
