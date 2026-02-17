# Deepstaging

> ⚠️ **Not ready for production use.** APIs are unstable and may change without notice.

Deepstaging is a C# source generator toolkit that eliminates boilerplate across four domains: **effect systems**, **strongly-typed IDs**, **configuration providers**, and **HTTP clients**. You annotate your types with attributes; the generators, analyzers, and code fixes do the rest — all at compile time, with zero reflection.

Built on [Deepstaging.Roslyn](https://github.com/deepstaging/roslyn).

## Features

### Effects

Turn service interfaces and `DbContext`s into composable [LanguageExt](https://github.com/louthy/language-ext) effect modules with automatic OpenTelemetry instrumentation.

```csharp
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

[EffectsModule(typeof(IEmailService), Name = "Email")]
public sealed partial class EmailEffects;

[Runtime]
[Uses(typeof(EmailEffects))]
public sealed partial class AppRuntime;
```

The generator produces `Eff<RT, A>` wrappers, capability interfaces, DI registration, and `Activity` spans — all wired together through the runtime.

### Strong IDs

Type-safe ID structs with configurable backing types and serialization converters.

```csharp
[StrongId]
public readonly partial struct UserId;

[StrongId(BackingType = BackingType.Int, Converters = IdConverters.All)]
public readonly partial struct OrderId;
```

Generated structs implement `IEquatable<T>`, `IParsable<T>`, `IComparable<T>`, and `ToString`. Opt into JSON, EF Core, Dapper, Newtonsoft, and `TypeConverter` support via the `Converters` flag.

### Configuration

Strongly-typed configuration providers with interfaces, DI registration, and JSON Schema generation.

```csharp
public sealed record SmtpSettings(string Host, int Port);

public class SmtpSecrets
{
    [Secret]
    public string Password { get; init; } = "";
}

[ConfigProvider(Section = "Smtp")]
[Exposes<SmtpSettings>]
[Exposes<SmtpSecrets>]
public sealed partial class SmtpConfigProvider;
```

The generator creates an `ISmtpConfigProvider` interface, binds properties from `IConfiguration`, and emits a DI extension method. `[Secret]` properties are separated into a dedicated secrets schema.

### HTTP Clients

Declarative HTTP clients generated from annotated partial classes.

```csharp
[HttpClient<ApiConfig>]
[BearerAuth]
public partial class UsersClient
{
    [Get("/users/{id}")]
    private partial User GetUser(int id);

    [Post("/users")]
    private partial User CreateUser([Body] CreateUserRequest request);

    [Get("/users")]
    private partial List<User> ListUsers([Query] int page, [Header("X-Request-Id")] string requestId);
}
```

Supports path parameters, query strings, headers, request bodies, and authentication (`[BearerAuth]`, `[ApiKeyAuth]`, `[BasicAuth]`). Generates a client implementation and an `IUsersClient` interface.

### Testing

`[TestRuntime<TRuntime>]` generates test-friendly runtime doubles from your production runtime, with fluent `.With*()` methods for injecting dependencies.

## Compile-Time Safety

Every feature ships with Roslyn analyzers and code fixes. Missing a `partial` modifier, targeting the wrong type, referencing a non-existent method — these are caught as compiler errors, not runtime surprises.

## Project Structure

```
src/
├── Deepstaging                    # Core attributes and enums
├── Deepstaging.Projection         # Roslyn analysis layer — attribute queries and pipeline models
├── Deepstaging.Generators         # Incremental source generators
├── Deepstaging.Analyzers          # Diagnostic analyzers
├── Deepstaging.CodeFixes          # Code fix providers
├── Deepstaging.Runtime            # Runtime support (OpenTelemetry, metrics)
├── Deepstaging.Testing            # Test support library (ITestRuntime, TestRuntimeAttribute)
└── Deepstaging.Testing.*          # Projection, generators, analyzers, and code fixes for testing
test/
├── Deepstaging.Tests              # Main test suite
└── Deepstaging.Testing.Tests      # Tests for the Testing support library
```

## Build & Test

```bash
# Build (also the lint step — warnings are errors)
dotnet build Deepstaging.slnx

# Run all tests
dotnet run --project test/Deepstaging.Tests -c Release

# Run tests by class
dotnet run --project test/Deepstaging.Tests -c Release --treenode-filter /*/*/StrongIdGeneratorTests/*

# Run a single test
dotnet run --project test/Deepstaging.Tests -c Release --treenode-filter /*/*/*/GeneratesGuidId_WithDefaultSettings
```

## License

**RPL-1.5** (Reciprocal Public License) — Real reciprocity, no loopholes.

You can use this code, modify it, and share it freely. But when you deploy it — internally or externally, as a service or within your company — you share your improvements back under the same license.

See [LICENSE](LICENSE) for the full legal text.
