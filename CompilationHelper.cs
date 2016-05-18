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
        public static SyntaxNode GetTypeDeclarationAsSyntaxNode(Compilation compilation, string fullTypeName)
        {
            var typeByMetadataName = compilation.Assembly.GetTypeByMetadataName(fullTypeName);
            System.Diagnostics.Debugger.Launch();
            var declaration = typeByMetadataName.DeclaringSyntaxReferences.Single();
            return declaration.GetSyntax();
        }
    }
}
