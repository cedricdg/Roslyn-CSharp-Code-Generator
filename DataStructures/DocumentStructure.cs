using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace CSharpCodeGenerator.DataStructures
{
    public class DocumentStructure : NodeStructure
    {
        internal readonly CompilationUnitSyntax Node;

        public DocumentStructure(CompilationUnitSyntax node)
        {
            Node = node;
        }

        public IEnumerable<ClassStructure> Classes {
            get
            {
                return from childNode in Node.DescendantNodes(n => !n.IsKind(SyntaxKind.ClassKeyword))
                    where childNode.IsKind(SyntaxKind.ClassDeclaration)
                    select new ClassStructure(childNode as ClassDeclarationSyntax);
            }
        }
    }
}
