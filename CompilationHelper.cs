using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

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
