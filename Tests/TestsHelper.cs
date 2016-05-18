using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpCodeGenerator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using NSpec;

namespace CSharpCodeGenerator.Tests
{
    static class TestsHelper
    {
        public static void LaunchDebugger()
        {
            System.Diagnostics.Debugger.Launch();
        }

        public static SyntaxNode RemoveTrivia(SyntaxNode node)
        {
            return node.WithoutTrivia();
        }
    }
}
