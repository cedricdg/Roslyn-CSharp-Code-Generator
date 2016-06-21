using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpCodeGenerator.DataStructures
{
    public class AttributeStructure : DataStructure
    {
        public readonly AttributeSyntax Node;

        internal AttributeStructure(AttributeSyntax node)
        {
            Node = node;
        }

        public string Identifier { get { return Node.Name.ToString(); } }
        public string[] Values { get
        {
            return Node.ArgumentList.Arguments.Select(a => a.ToString()).ToArray();
        } }
    }
}
