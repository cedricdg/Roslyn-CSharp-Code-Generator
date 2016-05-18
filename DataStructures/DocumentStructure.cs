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

        public ClassDeclarationSyntax[] Classes {
            get
            {
                return _node.Members
                    .Where(m => m.IsKind(SyntaxKind.ClassDeclaration))
                    .Cast<ClassDeclarationSyntax>()
                    .ToArray();
            }
        }
    }
}
