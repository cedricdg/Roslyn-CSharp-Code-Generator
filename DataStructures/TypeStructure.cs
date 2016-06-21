using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpCodeGenerator.DataStructures
{
    public class TypeStructure : DataStructure
    {
        public readonly TypeSyntax Node;

        internal TypeStructure(TypeSyntax node)
        {
            Node = node;
        }

        public string Name => Node.ToString();
    }
}
