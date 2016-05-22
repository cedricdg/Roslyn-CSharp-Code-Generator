using System.Linq;
using CSharpCodeGenerator.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSpec;

namespace CSharpCodeGenerator.Tests.DataStructures
{
    class describe_AttributeStructure : nspec
    {
        void when_wrapping()
        {
            it["returns correct attribute identifier"] = () =>
            {
                var attributeIdentifier = "AttributeIdentifier";
                var tree = CSharpSyntaxTree.ParseText($"[{attributeIdentifier}] class Testclass {{}}");

                var attributeNode =
                    tree.GetRoot().DescendantNodes()
                                        .Where(n => n.IsKind(SyntaxKind.Attribute))
                                        .Select(n => n as AttributeSyntax)
                                        .Single();
                var attributeStructure = new AttributeStructure(attributeNode);

                attributeStructure.Identifier.should_be(attributeIdentifier);
            };

            it["returns correct attribute identifier"] = () =>
            {
                var attributeIdentifier = "AttributeIdentifier";
                var attributeValue1 = "\"AttributeValue\"";
                var attributeValue2 = "AttributeValueTest";
                var tree = CSharpSyntaxTree.ParseText($"[{attributeIdentifier}({attributeValue1}, {attributeValue2})] class Testclass {{}}");

                var attributeNode =
                    tree.GetRoot().DescendantNodes()
                                        .Where(n => n.IsKind(SyntaxKind.Attribute))
                                        .Select(n => n as AttributeSyntax)
                                        .Single();
                var attributeStructure = new AttributeStructure(attributeNode);

                attributeStructure.Values.Length.should_be(2);
                attributeStructure.Values[0].should_be(attributeValue1);
                attributeStructure.Values[1].should_be(attributeValue2);
            };
        }
    }
}
