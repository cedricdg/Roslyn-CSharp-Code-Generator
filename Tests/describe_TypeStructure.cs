using System.Linq;
using CSharpCodeGenerator.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSpec;

namespace CSharpCodeGenerator.Tests.DataStructures
{
    class describe_TypeStructure : nspec
    {
        void when_wrapping()
        {
            it["returns correct type name"] = () =>
            {
                var typeName = "CostumType";
                var tree = CSharpSyntaxTree.ParseText($"class Testclass {{public static {typeName} fieldName;}}");
                
                var typeStructure = new DocumentStructure(tree.GetCompilationUnitRoot()).Classes.First().Fields.First().DeclarationType;
                
                typeStructure.Name.should_be(typeName);
            };
        }
    }
}
