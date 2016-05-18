using System.Linq;
using CSharpCodeGenerator.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSpec;

namespace CSharpCodeGenerator.Tests.DataStructures
{
    class describe_ClassStructure : nspec
    {
        void when_creating()
        {
            context["get fields"] = () =>
            {
                it["returns field node for field declaration"] = () =>
                {
                    var tree = CSharpSyntaxTree.ParseText("class A { private float _b; public bool C;}");
                    var expectedFieldDeclarations = tree.GetRoot().DescendantNodes()
                                                        .Where(n => n.IsKind(SyntaxKind.FieldDeclaration)).ToArray();
                    var classDeclaration = tree.GetRoot().DescendantNodes()
                                                .Single(e => e.IsKind(SyntaxKind.ClassDeclaration)) as ClassDeclarationSyntax;
                    var classStructure = new ClassStructure(classDeclaration);

                    classStructure.Fields.should_not_be_null();
                    classStructure.Fields.Length.should_be(2);
                    classStructure.Fields[0].should_be(expectedFieldDeclarations[0]);
                    classStructure.Fields[1].should_be(expectedFieldDeclarations[1]);
                };
            };
            
            xit["returns namespace of class"] = () =>
            {
                var tree = CSharpSyntaxTree.ParseText("class A { private float _b; public bool C;}");
                var expectedFieldDeclarations = tree.GetRoot().DescendantNodes()
                                                    .Where(n => n.IsKind(SyntaxKind.FieldDeclaration)).ToArray();
                var classDeclaration = tree.GetRoot().DescendantNodes()
                                            .Single(e => e.IsKind(SyntaxKind.ClassDeclaration)) as ClassDeclarationSyntax;
                var classStructure = new ClassStructure(classDeclaration);

                classStructure.Fields.should_not_be_null();
                classStructure.Fields.Length.should_be(2);
                classStructure.Fields[0].should_be(expectedFieldDeclarations[0]);
                classStructure.Fields[1].should_be(expectedFieldDeclarations[1]);
            };
            
        }
    }
}
