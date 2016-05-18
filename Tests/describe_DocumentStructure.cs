using System.Linq;
using CSharpCodeGenerator.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NSpec;

namespace CSharpCodeGenerator.Tests.DataStructures
{
    class describe_DocumentStructure : nspec
    {
        void when_wrapping()
        {
            context["get classes"] = () =>
            {
                it["returns single class"] = () =>
                {
                    var tree = CSharpSyntaxTree.ParseText("class Testclass {}");
                    var document = new DocumentStructure(tree.GetCompilationUnitRoot());
                    var expectedNode =
                        tree.GetRoot().DescendantNodes().Single(n => n.IsKind(SyntaxKind.ClassDeclaration));
                    document.Classes.Length.should_be(1);
                    document.Classes.Single().should_be(expectedNode);
                };

                it["returns multiple classes"] = () =>
                {
                    var tree = CSharpSyntaxTree.ParseText("class Testclass {}\nclass Testclass {}");
                    var document = new DocumentStructure(tree.GetCompilationUnitRoot());
                    var expectedNodes =
                        tree.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.ClassDeclaration)).ToArray();
                    document.Classes.Length.should_be(2);
                    document.Classes[0].should_be(expectedNodes[0]);
                    document.Classes[1].should_be(expectedNodes[1]);
                };
            };
        }
    }
}
