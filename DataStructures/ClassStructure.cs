﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public FieldStructure[] Fields
        {
            get { return _node.ChildNodes().Where(n => n.IsKind(SyntaxKind.FieldDeclaration)).Select(n => new FieldStructure(n as FieldDeclarationSyntax)).ToArray(); }
        }

        public ClassDeclarationSyntax Node => _node;

        public ModifierFlags ModifierFlags => _node.Modifiers.GetModifierFromTokenList();

        public AccessModifier AccessModifier => Node.Modifiers.GetAccessModifierFromTokenList();

        public string FullClassName => Namespace == string.Empty ? ClassName : $"{Namespace}.{ClassName}";

        public string ClassName => _node.Identifier.ToString();

        public string Namespace => GetNameSpaceFromParentsRecursive(_node);

        public AttributeStructure[] Attributes
        {
            get
            {
                var attributes = new List<AttributeStructure>();
                foreach (var attributeLists in _node.AttributeLists)
                {
                    foreach (var attribute in attributeLists.Attributes)
                    {
                        attributes.Add(new AttributeStructure(attribute));
                    }
                }

                return attributes.ToArray();
                
            }
        }


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

            return _node.Equals(otherClassStruct._node);
        }
    }

}
