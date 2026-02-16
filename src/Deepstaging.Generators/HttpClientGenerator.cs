// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators;

using HttpClient;

/// <summary>
/// Source generator that creates typed HTTP client implementations.
/// </summary>
[Generator]
public sealed class HttpClientGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register for non-generic HttpClientAttribute
        var models = context.ForAttribute<HttpClientAttribute>()
            .Map(static (ctx, _) => ctx.TargetSymbol.AsValidNamedType().QueryHttpClient());

        context.RegisterSourceOutput(models, static (ctx, model) =>
        {
            var hint = new HintName(model.Namespace);

            // Generate client class
            model.WriteClient()
                .AddSourceTo(ctx, hint.Filename(model.TypeName));

            // Generate interface
            model.WriteInterface()
                .AddSourceTo(ctx, hint.Filename(model.InterfaceName));

            // Generate request records for each method
            foreach (var request in model.Requests)
            {
                request.WriteRequest(model)
                    .AddSourceTo(ctx, hint.Filename(request.RequestTypeName));
            }
        });
    }
}
