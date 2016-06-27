using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpCodeGenerator.DataStructures
{
    public class AttributeStructure : NodeStructure
    {
        public readonly AttributeSyntax Node;

        internal AttributeStructure(AttributeSyntax node)
        {
            Node = node;
        }

        public string Identifier => Node.Name.ToString();

        public string[] Values => Node.ArgumentList.Arguments.Select(a => a.ToString()).ToArray();
    }
}
