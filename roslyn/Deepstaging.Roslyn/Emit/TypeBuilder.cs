namespace Deepstaging.Roslyn.Emit;

/// <summary>
/// Fluent builder for type declarations (classes, interfaces, structs, records).
/// Supports adding members (properties, methods, fields, constructors) and emitting compilable C# code.
/// Immutable - each method returns a new instance.
/// </summary>
public readonly struct TypeBuilder
{
    private readonly string _name;
    private readonly TypeKind _kind;
    private readonly string? _namespace;
    private readonly Accessibility _accessibility;
    private readonly bool _isStatic;
    private readonly bool _isAbstract;
    private readonly bool _isSealed;
    private readonly bool _isPartial;
    private readonly ImmutableArray<string> _usings;
    private readonly ImmutableArray<PropertyBuilder> _properties;
    private readonly ImmutableArray<FieldBuilder> _fields;
    private readonly ImmutableArray<MethodBuilder> _methods;
    private readonly ImmutableArray<ConstructorBuilder> _constructors;
    private readonly ImmutableArray<TypeBuilder> _nestedTypes;
    private readonly ImmutableArray<string> _interfaces;
    private readonly ImmutableArray<AttributeBuilder> _attributes;
    private readonly XmlDocumentationBuilder? _xmlDoc;

    private TypeBuilder(
        string name,
        TypeKind kind,
        string? @namespace,
        Accessibility accessibility,
        bool isStatic,
        bool isAbstract,
        bool isSealed,
        bool isPartial,
        ImmutableArray<string> usings,
        ImmutableArray<PropertyBuilder> properties,
        ImmutableArray<FieldBuilder> fields,
        ImmutableArray<MethodBuilder> methods,
        ImmutableArray<ConstructorBuilder> constructors,
        ImmutableArray<TypeBuilder> nestedTypes,
        ImmutableArray<string> interfaces,
        ImmutableArray<AttributeBuilder> attributes,
        XmlDocumentationBuilder? xmlDoc)
    {
        _name = name;
        _kind = kind;
        _namespace = @namespace;
        _accessibility = accessibility;
        _isStatic = isStatic;
        _isAbstract = isAbstract;
        _isSealed = isSealed;
        _isPartial = isPartial;
        _usings = usings.IsDefault ? ImmutableArray<string>.Empty : usings;
        _properties = properties.IsDefault ? ImmutableArray<PropertyBuilder>.Empty : properties;
        _fields = fields.IsDefault ? ImmutableArray<FieldBuilder>.Empty : fields;
        _methods = methods.IsDefault ? ImmutableArray<MethodBuilder>.Empty : methods;
        _constructors = constructors.IsDefault ? ImmutableArray<ConstructorBuilder>.Empty : constructors;
        _nestedTypes = nestedTypes.IsDefault ? ImmutableArray<TypeBuilder>.Empty : nestedTypes;
        _interfaces = interfaces.IsDefault ? ImmutableArray<string>.Empty : interfaces;
        _attributes = attributes.IsDefault ? ImmutableArray<AttributeBuilder>.Empty : attributes;
        _xmlDoc = xmlDoc;
    }

    #region Properties

    /// <summary>
    /// Gets the type name.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the type kind (class, interface, struct).
    /// </summary>
    public TypeKind Kind => _kind;

    #endregion

    #region Factory Methods

    /// <summary>
    /// Creates a class type builder.
    /// </summary>
    /// <param name="name">The class name (e.g., "Customer", "Repository").</param>
    public static TypeBuilder Class(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type name cannot be null or empty.", nameof(name));

        return new TypeBuilder(
            name,
            TypeKind.Class,
            @namespace: null,
            Accessibility.Public,
            isStatic: false,
            isAbstract: false,
            isSealed: false,
            isPartial: false,
            ImmutableArray<string>.Empty,
            ImmutableArray<PropertyBuilder>.Empty,
            ImmutableArray<FieldBuilder>.Empty,
            ImmutableArray<MethodBuilder>.Empty,
            ImmutableArray<ConstructorBuilder>.Empty,
            ImmutableArray<TypeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            ImmutableArray<AttributeBuilder>.Empty,
            xmlDoc: null);
    }

    /// <summary>
    /// Creates an interface type builder.
    /// </summary>
    /// <param name="name">The interface name (e.g., "IRepository", "IService").</param>
    public static TypeBuilder Interface(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type name cannot be null or empty.", nameof(name));

        return new TypeBuilder(
            name,
            TypeKind.Interface,
            @namespace: null,
            Accessibility.Public,
            isStatic: false,
            isAbstract: false,
            isSealed: false,
            isPartial: false,
            ImmutableArray<string>.Empty,
            ImmutableArray<PropertyBuilder>.Empty,
            ImmutableArray<FieldBuilder>.Empty,
            ImmutableArray<MethodBuilder>.Empty,
            ImmutableArray<ConstructorBuilder>.Empty,
            ImmutableArray<TypeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            ImmutableArray<AttributeBuilder>.Empty,
            xmlDoc: null);
    }

    /// <summary>
    /// Creates a struct type builder.
    /// </summary>
    /// <param name="name">The struct name (e.g., "Point", "Vector").</param>
    public static TypeBuilder Struct(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type name cannot be null or empty.", nameof(name));

        return new TypeBuilder(
            name,
            TypeKind.Struct,
            @namespace: null,
            Accessibility.Public,
            isStatic: false,
            isAbstract: false,
            isSealed: false,
            isPartial: false,
            ImmutableArray<string>.Empty,
            ImmutableArray<PropertyBuilder>.Empty,
            ImmutableArray<FieldBuilder>.Empty,
            ImmutableArray<MethodBuilder>.Empty,
            ImmutableArray<ConstructorBuilder>.Empty,
            ImmutableArray<TypeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            ImmutableArray<AttributeBuilder>.Empty,
            xmlDoc: null);
    }

    /// <summary>
    /// Creates a record type builder.
    /// </summary>
    /// <param name="name">The record name (e.g., "Customer", "Person").</param>
    public static TypeBuilder Record(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type name cannot be null or empty.", nameof(name));

        return new TypeBuilder(
            name,
            TypeKind.Class, // Record is a special kind of class
            @namespace: null,
            Accessibility.Public,
            isStatic: false,
            isAbstract: false,
            isSealed: false,
            isPartial: false,
            ImmutableArray<string>.Empty,
            ImmutableArray<PropertyBuilder>.Empty,
            ImmutableArray<FieldBuilder>.Empty,
            ImmutableArray<MethodBuilder>.Empty,
            ImmutableArray<ConstructorBuilder>.Empty,
            ImmutableArray<TypeBuilder>.Empty,
            ImmutableArray<string>.Empty,
            ImmutableArray<AttributeBuilder>.Empty,
            xmlDoc: null);
    }

    /// <summary>
    /// Creates a type builder by parsing a C# type signature.
    /// </summary>
    /// <param name="signature">The type signature (e.g., "public class CustomerService : IService").</param>
    /// <returns>A configured TypeBuilder with parsed kind, modifiers, and base types.</returns>
    /// <exception cref="ArgumentException">Thrown when the signature cannot be parsed.</exception>
    /// <example>
    /// <code>
    /// // Class with interface
    /// var builder = TypeBuilder.Parse("public class CustomerService : IService, IDisposable");
    /// 
    /// // Sealed class
    /// var builder = TypeBuilder.Parse("public sealed class CacheEntry");
    /// 
    /// // Abstract class
    /// var builder = TypeBuilder.Parse("public abstract class BaseHandler");
    /// 
    /// // Partial record
    /// var builder = TypeBuilder.Parse("public partial record OrderDto");
    /// </code>
    /// </example>
    public static TypeBuilder Parse(string signature) => SignatureParser.ParseType(signature);

    #endregion

    #region Namespace & Usings

    /// <summary>
    /// Sets the namespace for the type.
    /// </summary>
    /// <param name="namespace">The namespace (e.g., "MyApp.Domain", "Company.Project").</param>
    public TypeBuilder InNamespace(string @namespace)
    {
        return new TypeBuilder(_name, _kind, @namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    /// <summary>
    /// Adds a using directive to the compilation unit.
    /// </summary>
    /// <param name="namespace">The namespace to use (e.g., "System", "System.Collections.Generic").</param>
    public TypeBuilder AddUsing(string @namespace)
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings.Add(@namespace), _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    #endregion

    #region Accessibility & Modifiers

    /// <summary>
    /// Sets the accessibility of the type.
    /// </summary>
    public TypeBuilder WithAccessibility(Accessibility accessibility)
    {
        return new TypeBuilder(_name, _kind, _namespace, accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    /// <summary>
    /// Marks the type as static.
    /// </summary>
    public TypeBuilder AsStatic()
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, true, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    /// <summary>
    /// Marks the type as abstract.
    /// </summary>
    public TypeBuilder AsAbstract()
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, true,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    /// <summary>
    /// Marks the type as sealed.
    /// </summary>
    public TypeBuilder AsSealed()
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            true, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    /// <summary>
    /// Marks the type as partial.
    /// </summary>
    public TypeBuilder AsPartial()
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, true, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    #endregion

    #region Interfaces

    /// <summary>
    /// Adds an interface to implement.
    /// </summary>
    /// <param name="interfaceName">The interface name (e.g., "IDisposable", "IEnumerable&lt;T&gt;").</param>
    public TypeBuilder Implements(string interfaceName)
    {
        if (string.IsNullOrWhiteSpace(interfaceName))
            throw new ArgumentException("Interface name cannot be null or empty.", nameof(interfaceName));

        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces.Add(interfaceName), _attributes, _xmlDoc);
    }

    /// <summary>
    /// Adds multiple interfaces to implement.
    /// </summary>
    /// <param name="interfaceNames">The interface names to implement.</param>
    public TypeBuilder Implements(params string[] interfaceNames)
    {
        var builder = this;
        foreach (var name in interfaceNames)
        {
            builder = builder.Implements(name);
        }
        return builder;
    }

    #endregion

    #region Add Members - Properties

    /// <summary>
    /// Adds a property with lambda configuration.
    /// </summary>
    public TypeBuilder AddProperty(string name, string type, Func<PropertyBuilder, PropertyBuilder> configure)
    {
        var property = configure(PropertyBuilder.For(name, type));
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties.Add(property), _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured property.
    /// </summary>
    public TypeBuilder AddProperty(PropertyBuilder property)
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties.Add(property), _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    #endregion

    #region Add Members - Fields

    /// <summary>
    /// Adds a field with lambda configuration.
    /// </summary>
    public TypeBuilder AddField(string name, string type, Func<FieldBuilder, FieldBuilder> configure)
    {
        var field = configure(FieldBuilder.For(name, type));
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields.Add(field), _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured field.
    /// </summary>
    public TypeBuilder AddField(FieldBuilder field)
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields.Add(field), _methods, _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    #endregion

    #region Add Members - Methods

    /// <summary>
    /// Adds a method with lambda configuration.
    /// </summary>
    public TypeBuilder AddMethod(string name, Func<MethodBuilder, MethodBuilder> configure)
    {
        var method = configure(MethodBuilder.For(name));
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods.Add(method), _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured method.
    /// </summary>
    public TypeBuilder AddMethod(MethodBuilder method)
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods.Add(method), _constructors, _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    #endregion

    #region Add Members - Constructors

    /// <summary>
    /// Adds a constructor with lambda configuration.
    /// </summary>
    public TypeBuilder AddConstructor(Func<ConstructorBuilder, ConstructorBuilder> configure)
    {
        var constructor = configure(ConstructorBuilder.For(_name));
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors.Add(constructor), _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured constructor.
    /// </summary>
    public TypeBuilder AddConstructor(ConstructorBuilder constructor)
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors.Add(constructor), _nestedTypes, _interfaces, _attributes, _xmlDoc);
    }

    #endregion

    #region Add Members - Nested Types

    /// <summary>
    /// Adds a nested type with lambda configuration.
    /// </summary>
    public TypeBuilder AddNestedType(string name, Func<TypeBuilder, TypeBuilder> configure)
    {
        var nestedType = configure(TypeBuilder.Class(name));
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes.Add(nestedType), _interfaces, _attributes, _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured nested type.
    /// </summary>
    public TypeBuilder AddNestedType(TypeBuilder nestedType)
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes.Add(nestedType), _interfaces, _attributes, _xmlDoc);
    }

    #endregion

    #region XML Documentation

    /// <summary>
    /// Sets the XML documentation for the type.
    /// </summary>
    /// <param name="configure">Configuration callback for the XML documentation.</param>
    public TypeBuilder WithXmlDoc(Func<XmlDocumentationBuilder, XmlDocumentationBuilder> configure)
    {
        var xmlDoc = configure(XmlDocumentationBuilder.Create());
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the type with a simple summary.
    /// </summary>
    /// <param name="summary">The summary text.</param>
    public TypeBuilder WithXmlDoc(string summary)
    {
        var xmlDoc = XmlDocumentationBuilder.WithSummary(summary);
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, xmlDoc);
    }

    /// <summary>
    /// Sets the XML documentation for the type from parsed XmlDocumentation.
    /// </summary>
    /// <param name="documentation">The parsed XML documentation to copy.</param>
    public TypeBuilder WithXmlDoc(XmlDocumentation documentation)
    {
        if (documentation.IsEmpty)
            return this;

        var xmlDoc = XmlDocumentationBuilder.From(documentation);
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes, xmlDoc);
    }

    #endregion

    #region Attributes

    /// <summary>
    /// Adds an attribute to the type.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Serializable", "Obsolete").</param>
    public TypeBuilder WithAttribute(string name)
    {
        var attribute = AttributeBuilder.For(name);
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes.Add(attribute), _xmlDoc);
    }

    /// <summary>
    /// Adds an attribute to the type with configuration.
    /// </summary>
    /// <param name="name">The attribute name (e.g., "Obsolete", "JsonConverter").</param>
    /// <param name="configure">Configuration callback for the attribute.</param>
    public TypeBuilder WithAttribute(string name, Func<AttributeBuilder, AttributeBuilder> configure)
    {
        var attribute = configure(AttributeBuilder.For(name));
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes.Add(attribute), _xmlDoc);
    }

    /// <summary>
    /// Adds a pre-configured attribute to the type.
    /// </summary>
    public TypeBuilder WithAttribute(AttributeBuilder attribute)
    {
        return new TypeBuilder(_name, _kind, _namespace, _accessibility, _isStatic, _isAbstract,
            _isSealed, _isPartial, _usings, _properties, _fields, _methods, _constructors, _nestedTypes, _interfaces, _attributes.Add(attribute), _xmlDoc);
    }

    #endregion

    #region Emit

    /// <summary>
    /// Emits the type to a compilation unit with default options.
    /// Uses syntax validation by default.
    /// </summary>
    public OptionalEmit Emit()
    {
        return Emit(EmitOptions.Default);
    }

    /// <summary>
    /// Emits the type to a compilation unit with specified options.
    /// </summary>
    public OptionalEmit Emit(EmitOptions options)
    {
        try
        {
            var compilationUnit = BuildCompilationUnit();

            // Format the code
            var formatted = FormatCode(compilationUnit, options);

            // Validate if requested
            if (options.ValidationLevel != ValidationLevel.None)
            {
                var diagnostics = Validate(formatted, options.ValidationLevel);
                if (diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
                {
                    return OptionalEmit.FromFailure(diagnostics);
                }

                return diagnostics.Any()
                    ? OptionalEmit.FromDiagnostics(compilationUnit, formatted, diagnostics)
                    : OptionalEmit.FromSuccess(compilationUnit, formatted);
            }

            return OptionalEmit.FromSuccess(compilationUnit, formatted);
        }
        catch (Exception ex)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(
                    "EMIT001",
                    "Emit failed",
                    $"Failed to emit type: {ex.Message}",
                    "Emit",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                Location.None);

            return OptionalEmit.FromFailure(ImmutableArray.Create(diagnostic));
        }
    }

    private CompilationUnitSyntax BuildCompilationUnit()
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();

        // Add usings
        foreach (var @using in _usings.Distinct())
        {
            UsingDirectiveSyntax usingDirective;
            
            if (@using.StartsWith("static ", StringComparison.Ordinal))
            {
                // Static using: "static LanguageExt.Prelude" -> using static LanguageExt.Prelude;
                var typeName = @using.Substring(7); // Remove "static " prefix
                usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeName))
                    .WithStaticKeyword(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            }
            else
            {
                usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(@using));
            }
            
            compilationUnit = compilationUnit.AddUsings(usingDirective);
        }

        // Build type declaration
        var typeDecl = BuildTypeDeclaration();

        // Wrap in namespace if specified
        if (!string.IsNullOrWhiteSpace(_namespace))
        {
            var namespaceDecl = SyntaxFactory.FileScopedNamespaceDeclaration(
                SyntaxFactory.ParseName(_namespace!))
                .AddMembers(typeDecl);

            compilationUnit = compilationUnit.AddMembers(namespaceDecl);
        }
        else
        {
            compilationUnit = compilationUnit.AddMembers(typeDecl);
        }

        // Add auto-generated header and nullable enable directive to the first token
        var headerComment = SyntaxFactory.Comment("// <auto-generated/>");
        var nullableDirective = SyntaxFactory.Comment("#nullable enable");
        var newLine = SyntaxFactory.EndOfLine(Environment.NewLine);
        var firstToken = compilationUnit.GetFirstToken();
        var newFirstToken = firstToken.WithLeadingTrivia(
            SyntaxFactory.TriviaList(headerComment, newLine, nullableDirective, newLine)
                .AddRange(firstToken.LeadingTrivia));
        compilationUnit = compilationUnit.ReplaceToken(firstToken, newFirstToken);

        return compilationUnit;
    }

    private TypeDeclarationSyntax BuildTypeDeclaration()
    {
        TypeDeclarationSyntax typeDecl = _kind switch
        {
            TypeKind.Class => SyntaxFactory.ClassDeclaration(_name),
            TypeKind.Interface => SyntaxFactory.InterfaceDeclaration(_name),
            TypeKind.Struct => SyntaxFactory.StructDeclaration(_name),
            _ => throw new InvalidOperationException($"Unsupported type kind: {_kind}")
        };

        // Add attributes
        if (_attributes.Length > 0)
        {
            var attributeLists = _attributes.Select(a => a.BuildList()).ToArray();
            typeDecl = typeDecl.WithAttributeLists(SyntaxFactory.List(attributeLists));
        }

        // Add modifiers
        var modifiers = new List<SyntaxKind>();
        modifiers.Add(AccessibilityToSyntaxKind(_accessibility));
        if (_isStatic) modifiers.Add(SyntaxKind.StaticKeyword);
        if (_isAbstract) modifiers.Add(SyntaxKind.AbstractKeyword);
        if (_isSealed) modifiers.Add(SyntaxKind.SealedKeyword);
        if (_isPartial) modifiers.Add(SyntaxKind.PartialKeyword);

        typeDecl = typeDecl.WithModifiers(
            SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token)));

        // Add base list (interfaces)
        if (_interfaces.Length > 0)
        {
            var baseTypes = _interfaces
                .Select(i => SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(i)))
                .ToArray<BaseTypeSyntax>();

            typeDecl = typeDecl.WithBaseList(
                SyntaxFactory.BaseList(SyntaxFactory.SeparatedList(baseTypes)));
        }

        // Add members in logical order: fields, constructors, properties, methods
        foreach (var field in _fields)
        {
            typeDecl = typeDecl.AddMembers(field.Build());
        }

        foreach (var constructor in _constructors)
        {
            typeDecl = typeDecl.AddMembers(constructor.Build());
        }

        foreach (var property in _properties)
        {
            typeDecl = typeDecl.AddMembers(property.Build());
        }

        foreach (var method in _methods)
        {
            typeDecl = typeDecl.AddMembers(method.Build());
        }

        foreach (var nestedType in _nestedTypes)
        {
            typeDecl = typeDecl.AddMembers(nestedType.BuildNestedTypeDeclaration());
        }

        // Add XML documentation
        if (_xmlDoc.HasValue && _xmlDoc.Value.HasContent)
        {
            var trivia = _xmlDoc.Value.Build();
            typeDecl = typeDecl.WithLeadingTrivia(trivia);
        }

        return typeDecl;
    }

    /// <summary>
    /// Builds the type as a nested type declaration (no namespace wrapper).
    /// </summary>
    internal TypeDeclarationSyntax BuildNestedTypeDeclaration()
    {
        return BuildTypeDeclaration();
    }

    private string FormatCode(CompilationUnitSyntax compilationUnit, EmitOptions options)
    {
        // First normalize whitespace for proper indentation
        var normalized = compilationUnit.NormalizeWhitespace(options.Indentation, options.EndOfLine);

        // Then add blank lines between members (NormalizeWhitespace strips them)
        var withBlankLines = AddBlankLinesBetweenMembers(normalized, options.EndOfLine);

        return withBlankLines.ToFullString();
    }

    private static CompilationUnitSyntax AddBlankLinesBetweenMembers(CompilationUnitSyntax root, string endOfLine)
    {
        return (CompilationUnitSyntax)new BlankLineRewriter(endOfLine).Visit(root);
    }

    private sealed class BlankLineRewriter(string endOfLine) : CSharpSyntaxRewriter
    {
        private readonly SyntaxTrivia _blankLine = SyntaxFactory.EndOfLine(endOfLine);

        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var visited = (ClassDeclarationSyntax?)base.VisitClassDeclaration(node);
            return visited == null ? null : AddBlankLines(visited);
        }

        public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node)
        {
            var visited = (StructDeclarationSyntax?)base.VisitStructDeclaration(node);
            return visited == null ? null : AddBlankLines(visited);
        }

        public override SyntaxNode? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var visited = (InterfaceDeclarationSyntax?)base.VisitInterfaceDeclaration(node);
            return visited == null ? null : AddBlankLines(visited);
        }

        public override SyntaxNode? VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            var visited = (RecordDeclarationSyntax?)base.VisitRecordDeclaration(node);
            return visited == null ? null : AddBlankLines(visited);
        }

        private T AddBlankLines<T>(T node) where T : TypeDeclarationSyntax
        {
            if (node.Members.Count < 2)
                return node;

            var newMembers = new List<MemberDeclarationSyntax>(node.Members.Count);

            for (int i = 0; i < node.Members.Count; i++)
            {
                var member = node.Members[i];

                if (i > 0)
                {
                    // Prepend a blank line to existing leading trivia
                    var existingTrivia = member.GetLeadingTrivia();
                    var newTrivia = existingTrivia.Insert(0, _blankLine);
                    member = member.WithLeadingTrivia(newTrivia);
                }

                newMembers.Add(member);
            }

            return (T)node.WithMembers(SyntaxFactory.List(newMembers));
        }
    }

    private ImmutableArray<Diagnostic> Validate(string code, ValidationLevel level)
    {
        if (level == ValidationLevel.None)
            return ImmutableArray<Diagnostic>.Empty;

        if (level == ValidationLevel.Syntax)
        {
            // Parse and check for syntax errors
            var tree = CSharpSyntaxTree.ParseText(code);
            var diagnostics = tree.GetDiagnostics();
            return diagnostics.ToImmutableArray();
        }

        // Semantic and Full validation require Compilation (Phase 2)
        // For now, return empty diagnostics
        return ImmutableArray<Diagnostic>.Empty;
    }

    private static SyntaxKind AccessibilityToSyntaxKind(Accessibility accessibility) =>
        accessibility switch
        {
            Accessibility.Public => SyntaxKind.PublicKeyword,
            Accessibility.Private => SyntaxKind.PrivateKeyword,
            Accessibility.Protected => SyntaxKind.ProtectedKeyword,
            Accessibility.Internal => SyntaxKind.InternalKeyword,
            _ => SyntaxKind.PublicKeyword
        };

    #endregion
}
