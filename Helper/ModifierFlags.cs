using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpCodeGenerator
{
    internal static class ModifierExtensions
    {
        private static readonly Dictionary<SyntaxKind, ModifierFlags> _syntaxKindModifierMapping = new Dictionary
            <SyntaxKind, ModifierFlags>
        {
            {SyntaxKind.AbstractKeyword, ModifierFlags.Abstract},
            {SyntaxKind.ReadOnlyKeyword, ModifierFlags.Readonly},
            {SyntaxKind.StaticKeyword, ModifierFlags.Static}
        };

        private static readonly Dictionary<SyntaxKind, AccessModifier> _syntaxKindAccessModifierMapping = new Dictionary
            <SyntaxKind, AccessModifier>
        {
            {SyntaxKind.PublicKeyword, AccessModifier.Public},
            {SyntaxKind.PrivateKeyword, AccessModifier.Private},
            {SyntaxKind.InternalKeyword, AccessModifier.Internal},
            {SyntaxKind.ProtectedKeyword, AccessModifier.Protected}
        };

        internal static ModifierFlags GetModifierFromTokenList(this SyntaxTokenList modifierList)
        {
            var modifierFlags = ModifierFlags.None;
            foreach (var modifier in modifierList)
            {
                if(_syntaxKindModifierMapping.ContainsKey(modifier.Kind()))
                modifierFlags = modifierFlags | _syntaxKindModifierMapping[modifier.Kind()];
            }
            return modifierFlags;
        }

        internal static AccessModifier GetAccessModifierFromTokenList(this SyntaxTokenList modifierList)
        {
            foreach (var modifier in modifierList)
            {
                if(_syntaxKindAccessModifierMapping.ContainsKey(modifier.Kind()))
                    return _syntaxKindAccessModifierMapping[modifier.Kind()];
            }
            return AccessModifier.None;
        }
    }

    // Define an Enum with FlagsAttribute.
    [FlagsAttribute]
    public enum ModifierFlags : short
    {
        None = 0,
        Abstract = 1,
        Static = 2,
        Readonly = 4,
        // Not implemented yet:
        Virtual = 8,
        Sealed = 16,
        Async = 32,
        Event = 64,
        Extern = 128,
        Const = 256,
        Override = 512,
        Partial = 1024,
        New = 2048,
        Unsafe = 4096,
        Volatile = 8192
    }

    public enum AccessModifier
    {
        None,
        Public,
        Private,
        Internal,
        Protected
    }
}
