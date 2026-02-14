# Deepstaging.Config.Analyzers

Diagnostic analyzers that validate correct usage of RoslynKit attributes. Uses
the [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn) framework and shares projections with the generators.

## Diagnostics

| ID     | Severity | Description                                              | Fix Available |
|--------|----------|----------------------------------------------------------|---------------|
| RK1001 | Error    | Type with `[GenerateWith]` must be declared as `partial` | ✅             |
| RK1002 | Error    | Type with `[AutoNotify]` must be declared as `partial`   | ✅             |
| RK1003 | Warning  | AutoNotify backing field should be `private`             | ✅             |

## Analyzers

| Analyzer                               | Diagnostic |
|----------------------------------------|------------|
| `GenerateWithAnalyzer`                 | RK1001     |
| `AutoNotifyMustBePartialAnalyzer`      | RK1002     |
| `AutoNotifyFieldMustBePrivateAnalyzer` | RK1003     |

## Architecture

Analyzers use the same Projection layer as generators, ensuring consistent interpretation of attributes:

```
Symbol → Projection Query → Validation → Diagnostic
```

## Related Projects

- [RoslynKit](../Deepstaging.Config/) - Attribute definitions
- [RoslynKit.Projection](../Deepstaging.Config.Projection/) - Shared queries and models
- [RoslynKit.CodeFixes](../Deepstaging.Config.CodeFixes/) - Code fixes for these diagnostics
- [RoslynKit.Tests](../Deepstaging.Config.Tests/) - Analyzer tests
- [Project README](../../README.md) - Full documentation

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](../../../LICENSE) for the full legal text.
