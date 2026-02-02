// Polyfills for netstandard2.0 to enable modern C# language features.
// These types are normally provided by the runtime in .NET 5+ but must be
// defined for netstandard2.0 targeting.

// ReSharper disable once CheckNamespace

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit;

    /// <summary>
    /// Indicates that a type has required members.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute;

    /// <summary>
    /// Specifies that this constructor sets all required members.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute(string featureName) : Attribute
    {
        public string FeatureName { get; } = featureName;
        public bool IsOptional { get; init; }
        public const string RefStructs = nameof(RefStructs);
        public const string RequiredMembers = nameof(RequiredMembers);
    }
}

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Specifies that this constructor sets all required members.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute;
}