using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpCodeGenerator.DataStructures
{

    public class ClassStructure : DataStructure
    {
        internal readonly ClassDeclarationSyntax Node;

        internal ClassStructure(ClassDeclarationSyntax node)
        {
            Node = node;
        }

        public IEnumerable<FieldStructure> Fields
        {
            get { return Node.ChildNodes().Where(n => n.IsKind(SyntaxKind.FieldDeclaration)).Select(n => new FieldStructure(n as FieldDeclarationSyntax)); }
        }

        public ModifierFlags ModifierFlags => Node.Modifiers.GetModifierFromTokenList();

        public AccessModifier AccessModifier => Node.Modifiers.GetAccessModifierFromTokenList();

        public string FullClassName => Namespace == string.Empty ? ClassName : $"{Namespace}.{ClassName}";

        public string ClassName => Node.Identifier.ToString();

        public string Namespace => GetNameSpaceFromParentsRecursive(Node);

        public IEnumerable<AttributeStructure> Attributes
        {
            get
            {
                foreach (var attributeLists in Node.AttributeLists)
                {
                    foreach (var attribute in attributeLists.Attributes)
                    {
                        yield return new AttributeStructure(attribute);
                    }
                }
            }
        }

        public IEnumerable<TypeStructure> BaseTypes => Node.BaseList.Types.Select(t => new TypeStructure(t.Type));


        private string GetNameSpaceFromParentsRecursive(ClassDeclarationSyntax node)
        {
            var result = string.Empty;

            var namespaceDeclaration = TryGetParentSyntax<NamespaceDeclarationSyntax>(node.Parent);

            while (namespaceDeclaration != null)
            {
                result = result == string.Empty ? namespaceDeclaration.Name.ToString() : $"{namespaceDeclaration.Name}.{result}";
                namespaceDeclaration = TryGetParentSyntax<NamespaceDeclarationSyntax>(namespaceDeclaration.Parent);
            }
            return result;
        }

        public static T TryGetParentSyntax<T>(SyntaxNode syntaxNode)
            where T : SyntaxNode
        {
            if (syntaxNode == null)
                return null;

            T result = syntaxNode as T;
            if (result != null)
                return result;

            return TryGetParentSyntax<T>(syntaxNode.Parent);
        }

        public override bool Equals(object obj)
        {
            var otherClassStruct = obj as ClassStructure;
            if (otherClassStruct == null)
            {
                return base.Equals(obj);
            }

            return Node.Equals(otherClassStruct.Node);
        }
    }

}
