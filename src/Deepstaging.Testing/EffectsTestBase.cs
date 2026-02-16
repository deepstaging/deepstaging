// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

#if NET10_0_OR_GREATER

namespace Deepstaging;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Base class for effects-based tests. Provides <see cref="CreateRuntime()"/> and
/// <see cref="CreateRuntime(Func{TRuntime, TRuntime})"/> factory methods
/// for creating test runtimes from the generated <c>[TestRuntime]</c> type.
/// </summary>
/// <typeparam name="THost">
/// The test host type, shared across tests via TUnit's <c>[ClassDataSource]</c>.
/// Subclasses expose this as a required property with the appropriate sharing scope.
/// </typeparam>
/// <typeparam name="TRuntime">
/// The generated test runtime type decorated with <c>[TestRuntime]</c>.
/// Must implement <see cref="ITestRuntime{TSelf}"/>.
/// </typeparam>
/// <example>
/// <code>
/// // 1. Define your test host
/// public class AppTestHost : IAsyncInitializer, IAsyncDisposable { ... }
///
/// // 2. Define your test base (adds the ClassDataSource attribute with concrete type)
/// public abstract class AppTestBase : EffectsTestBase&lt;AppTestHost, TestRuntime&gt;
/// {
///     [ClassDataSource&lt;AppTestHost&gt;(Shared = SharedType.PerTestSession)]
///     public required AppTestHost Host { get; init; }
/// }
///
/// // 3. Write tests
/// public class PeopleTests : AppTestBase
/// {
///     [Test]
///     public async Task MyTest()
///     {
///         var runtime = CreateRuntime()
///             .WithMyDbContext(Host.Db);
///         ...
///     }
/// }
/// </code>
/// </example>
public abstract class EffectsTestBase<THost, TRuntime>
    where THost : class
    where TRuntime : ITestRuntime<TRuntime>
{
    /// <summary>
    /// Creates a new <typeparamref name="TRuntime"/> instance with all capabilities unconfigured.
    /// Chain <c>.With*()</c> methods to configure capabilities before use.
    /// </summary>
    protected virtual TRuntime CreateRuntime() => TRuntime.Create();
    
    /// <summary>
    /// Creates a new <typeparamref name="TRuntime"/> instance and applies the provided configuration function.
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    protected TRuntime CreateRuntime(Func<TRuntime, TRuntime> configure) => 
        configure(CreateRuntime());
}

#endif
