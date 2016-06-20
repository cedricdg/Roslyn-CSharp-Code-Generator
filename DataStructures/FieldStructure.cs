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

    }
}
