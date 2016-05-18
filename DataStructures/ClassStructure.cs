using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpCodeGenerator.DataStructures
{
    public class ClassStructure : DataStructure
    {
        private readonly ClassDeclarationSyntax _node;

        public ClassStructure(ClassDeclarationSyntax node)
        {
            _node = node;
        }

        public FieldDeclarationSyntax[] Fields {
            get { return _node.ChildNodes().Where(n => n.IsKind(SyntaxKind.FieldDeclaration)).Cast<FieldDeclarationSyntax>().ToArray(); }
        }

        public ClassDeclarationSyntax Node
        {
            get { return _node; }
        }
    }
}
