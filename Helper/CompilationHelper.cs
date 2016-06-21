using System.Linq;
using Microsoft.CodeAnalysis;

namespace CSharpCodeGenerator.Extensions
{
    public static class CompilationHelper
    {
        public static SyntaxNode GetTypeDeclarationAsSyntaxNode(string fullTypeName, Compilation compilation)
        {
            var typeByMetadataName = compilation.Assembly.GetTypeByMetadataName(fullTypeName);
            if (typeByMetadataName == null)
            {
                return null;
            }

            var declaration = typeByMetadataName.DeclaringSyntaxReferences.Single();
            return declaration.GetSyntax();
        }

        public static SyntaxNode GetTypeDeclarationAsSyntaxNode( string fullTypeName, params Compilation[] compilations)
        {
            foreach (var compilation in compilations)
            {
                var declaration = GetTypeDeclarationAsSyntaxNode(fullTypeName, compilation);
                if (declaration != null)
                {
                    return declaration;
                }
            }
            return null;
        }
    }
}
