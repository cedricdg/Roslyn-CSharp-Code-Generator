using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpCodeGenerator.DataStructures
{
    public class DocumentStructure : DataStructure
    {
        protected readonly CompilationUnitSyntax _node;

        public DocumentStructure(CompilationUnitSyntax node)
        {
            _node = node;
        }

        public ClassStructure[] Classes {
            get
            {
                return _node.DescendantNodes()
                    .Where(m => m.IsKind(SyntaxKind.ClassDeclaration))
                    .Select((node, index) => new ClassStructure(node as ClassDeclarationSyntax))
                    .ToArray();
            }
        }
    }
}
