using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpCodeGenerator.DataStructures
{
    public class FieldStructure : DataStructure
    {
        public readonly FieldDeclarationSyntax Node;
        public FieldStructure(FieldDeclarationSyntax node)
        {
            Node = node;
        }

        public string Identifier => Node.Declaration.Variables.First().Identifier.ValueText;

        public ModifierFlags ModifierFlags => Node.Modifiers.GetModifierFromTokenList();

        public AccessModifier AccessModifier => Node.Modifiers.GetAccessModifierFromTokenList();

        public string GetFullMetadataNameByCompilation(Compilation compilation)
        {
            var semanticModel = compilation.GetSemanticModel(compilation.SyntaxTrees.First());
            var typeInfo = semanticModel.GetSpeculativeTypeInfo(Node.SpanStart, Node.Declaration.Type, SpeculativeBindingOption.BindAsTypeOrNamespace);
            var typeSymbol = typeInfo.Type;

            var symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
            string fullyQualifiedName = typeSymbol.ToDisplayString(symbolDisplayFormat);
            return fullyQualifiedName;
        }
    }
}
