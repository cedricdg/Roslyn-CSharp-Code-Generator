using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpCodeGenerator.DataStructures
{
    public class FieldStructure : NodeStructure
    {
        internal readonly FieldDeclarationSyntax Node;
        internal FieldStructure(FieldDeclarationSyntax node)
        {
            Node = node;
        }

        public string Identifier => Node.Declaration.Variables.First().Identifier.ValueText;

        public TypeStructure DeclarationType => new TypeStructure(Node.Declaration.Type);

        public ModifierFlags ModifierFlags => Node.Modifiers.GetModifierFromTokenList();

        public AccessModifier AccessModifier => Node.Modifiers.GetAccessModifierFromTokenList();

    }
}
