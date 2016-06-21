using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace CSharpCodeGenerator.DataStructures
{
    public class DocumentStructure : DataStructure
    {
        internal readonly CompilationUnitSyntax Node;

        public DocumentStructure(CompilationUnitSyntax node)
        {
            Node = node;
        }

        public IEnumerable<ClassStructure> Classes {
            get
            {
                return Node.DescendantNodes(n => !n.IsKind(SyntaxKind.ClassKeyword))
                    .Where(m => m.IsKind(SyntaxKind.ClassDeclaration))
                    .Select((node, index) => new ClassStructure(node as ClassDeclarationSyntax));
            }
        }
    }
}
