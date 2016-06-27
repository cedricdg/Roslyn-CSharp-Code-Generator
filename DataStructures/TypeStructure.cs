using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpCodeGenerator.DataStructures
{
    public class TypeStructure : NodeStructure
    {
        public readonly TypeSyntax Node;

        internal TypeStructure(TypeSyntax node)
        {
            Node = node;
        }

        public string Name => Node.ToString();
    }
}
