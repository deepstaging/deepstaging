# Deepstaging.Config.CodeFixes

Code fix providers for RoslynKit analyzer diagnostics. Each fix addresses a specific diagnostic ID.

## Code Fixes

| Provider                           | Fixes          | Action                                   |
|------------------------------------|----------------|------------------------------------------|
| `MakePartialClassCodeFixProvider`  | RK1001, RK1002 | Adds `partial` modifier to class         |
| `MakePartialStructCodeFixProvider` | RK1001         | Adds `partial` modifier to struct        |
| `MakePrivateCodeFixProvider`       | RK1003         | Changes field accessibility to `private` |

## Example

Before fix (RK1001):

```csharp
[GenerateWith]
public class Person  // Error: must be partial
{
    public string Name { get; init; }
}
```

After fix:

```csharp
[GenerateWith]
public partial class Person
{
    public string Name { get; init; }
}
```

## Related Projects

- [RoslynKit.Analyzers](../Deepstaging.Config.Analyzers/) - Analyzers that report these diagnostics
- [RoslynKit.Tests](../Deepstaging.Config.Tests/) - Code fix tests
- [Project README](../../README.md) - Full documentation

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
