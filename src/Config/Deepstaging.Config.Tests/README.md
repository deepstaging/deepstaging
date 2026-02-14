# Deepstaging.Config.Tests

Unit tests for RoslynKit generators, analyzers, and code fixes using [TUnit](https://github.com/thomhurst/TUnit)
and [Deepstaging.Roslyn.Testing](https://github.com/deepstaging/roslyn).

## Test Files

| File                                 | Tests                             |
|--------------------------------------|-----------------------------------|
| `WithMethodsGeneratorTests.cs`       | `[GenerateWith]` source generator |
| `GenerateWithAnalyzerTests.cs`       | RK1001 diagnostic                 |
| `MakePartialCodeFixProviderTests.cs` | `partial` modifier code fixes     |

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "FullyQualifiedName~WithMethodsGeneratorTests"
```

## Writing Tests

Tests use the Deepstaging.Roslyn.Testing framework:

```csharp
[Test]
public async Task GeneratesWithMethod()
{
    var source = """
        [GenerateWith]
        public partial class Person
        {
            public string Name { get; init; }
        }
        """;
    
    await Verify.Generator<WithMethodsGenerator>()
        .WithSource(source)
        .HasGeneratedSource("Person.g.cs", expected);
}
```

## Related Projects

- [RoslynKit.Generators](../Deepstaging.Config.Generators/) - Tested generators
- [RoslynKit.Analyzers](../Deepstaging.Config.Analyzers/) - Tested analyzers
- [RoslynKit.CodeFixes](../Deepstaging.Config.CodeFixes/) - Tested code fixes
- [Project README](../../README.md) - Full documentation

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service
or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
