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
        private ClassStructure CreateClassStructureFromTree(SyntaxTree tree)
        {
            var classDeclaration = tree.GetRoot().DescendantNodes()
                .Single(e => e.IsKind(SyntaxKind.ClassDeclaration)) as ClassDeclarationSyntax;
            return new ClassStructure(classDeclaration);
        }

        void when_creating()
        {
            context["get fields"] = () =>
            {
                it["returns field node for field declaration"] = () =>
                {
                    var tree = CSharpSyntaxTree.ParseText("class A { private float _b; public bool C;}");
                    var expectedFieldDeclarations = tree.GetRoot().DescendantNodes()
                                                        .Where(n => n.IsKind(SyntaxKind.FieldDeclaration)).ToArray();

                    var classStructure = CreateClassStructureFromTree(tree);

                    classStructure.Fields.should_not_be_null();
                    classStructure.Fields.Count().should_be(2);
                    classStructure.Fields.ElementAt(0).Node.should_be(expectedFieldDeclarations[0]);
                    classStructure.Fields.ElementAt(1).Node.should_be(expectedFieldDeclarations[1]);
                };
            };

            context["get modifier"] = () =>
            {
                it["returns None when no modifier"] = () =>
                {
                    var tree = CSharpSyntaxTree.ParseText("class A {}");
                    var expectedFieldDeclarations = tree.GetRoot().DescendantNodes()
                        .Where(n => n.IsKind(SyntaxKind.AbstractKeyword)).ToArray();

                    var classStructure = CreateClassStructureFromTree(tree);

                    classStructure.ModifierFlags.should_be(ModifierFlags.None);
                };
                it["returns abstract modifier"] = () =>
                {
                    var tree = CSharpSyntaxTree.ParseText("abstract class A {}");
                    var expectedFieldDeclarations = tree.GetRoot().DescendantNodes()
                        .Where(n => n.IsKind(SyntaxKind.AbstractKeyword)).ToArray();

                    var classStructure = CreateClassStructureFromTree(tree);

                    classStructure.ModifierFlags.should_be(ModifierFlags.Abstract);
                };
            };

            context["get namespace"] = () =>
            {
                it["returns empty namespace when class is not wrapped in namespace"] = () =>
                {
                    var tree = CSharpSyntaxTree.ParseText("class A {}");

                    var classStructure = CreateClassStructureFromTree(tree);

                    classStructure.Namespace.should_be(string.Empty);
                };

                it["returns namespace for class"] = () =>
                {
                    var nameSpace = "Test.NameSpace.LongTestWordWITHWeirdCAPITALIZATION";
                    var tree = CSharpSyntaxTree.ParseText($"namespace {nameSpace} {{ class A {{}} }}");

                    var classStructure = CreateClassStructureFromTree(tree);

                    classStructure.Namespace.should_be(nameSpace);
                };

                it["returns nested namespace for class"] = () =>
                {
                    var nameSpace1 = "Test.NameSpace1";
                    var nameSpace2 = "NameSpace2";
                    var tree = CSharpSyntaxTree.ParseText($"namespace {nameSpace1} {{ namespace {nameSpace2} {{ class A {{}} }} }}");

                    var classStructure = CreateClassStructureFromTree(tree);

                    classStructure.Namespace.should_be($"{nameSpace1}.{nameSpace2}");
                };
            };

            context["get classnames"] = () =>
            {
                it["returns full name including namespace and classname for class"] = () =>
                {
                    var nameSpace1 = "Test.NameSpace";
                    var className = "ClassName";
                    var tree = CSharpSyntaxTree.ParseText($"namespace {nameSpace1} {{ class {className} {{  }} }}");

                    var classStructure = CreateClassStructureFromTree(tree);

                    classStructure.FullClassName.should_be($"{nameSpace1}.{className}");
                };

                it["returns class name"] = () =>
                {
                    var nameSpace1 = "Test.NameSpace";
                    var className = "ClassName";
                    var tree = CSharpSyntaxTree.ParseText($"namespace {nameSpace1} {{ class {className} {{  }} }}");

                    var classStructure = CreateClassStructureFromTree(tree);

                    classStructure.ClassName.should_be(className);
                };
            };

            context["get attributes"] = () =>
            {
                it["returns empty array when class has no attributes"] = () =>
                {
                    var tree = CSharpSyntaxTree.ParseText($"class A {{  }}");
                    var classStructure = CreateClassStructureFromTree(tree);

                    classStructure.Attributes.Count().should_be(0);
                };

                it["returns array with correct attribute nodes"] = () =>
                {
                    var tree = CSharpSyntaxTree.ParseText("[Pool(\"PoolA\"), Pool(\"PoolB\"), IsTestAttribute]\n class A {{  }}");
                    var classStructure = CreateClassStructureFromTree(tree);

                    var expectedAttributeNodes = tree.GetRoot().DescendantNodes().Where(n => n.IsKind(SyntaxKind.Attribute));

                    classStructure.Attributes.Count().should_be(3);
                    classStructure.Attributes.should_contain(attr => attr.Node.Equals(expectedAttributeNodes.ElementAt(0)));
                    classStructure.Attributes.should_contain(attr => attr.Node.Equals(expectedAttributeNodes.ElementAt(1)));
                    classStructure.Attributes.should_contain(attr => attr.Node.Equals(expectedAttributeNodes.ElementAt(2)));
                };
            };

        }
    }
}