// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.Dispatch.Models;

/// <summary>
/// Model for a dispatch module discovered via <see cref="DispatchModuleAttribute"/>.
/// Contains all information needed to generate typed dispatch methods.
/// </summary>
[PipelineModel]
public sealed record DispatchModel
{
    /// <summary>
    /// The name of the partial class decorated with [DispatchModule].
    /// </summary>
    public required string ContainerName { get; init; }

    /// <summary>
    /// The namespace of the dispatch class.
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The accessibility modifier (e.g., "public", "internal").
    /// </summary>
    public required string Accessibility { get; init; }

    /// <summary>
    /// Whether auto-commit is enabled for command dispatch.
    /// </summary>
    public bool AutoCommit { get; init; } = true;

    /// <summary>
    /// The command handler groups discovered in the assembly.
    /// </summary>
    public EquatableArray<DispatchHandlerGroupModel> CommandHandlers { get; init; } = [];

    /// <summary>
    /// The query handler groups discovered in the assembly.
    /// </summary>
    public EquatableArray<DispatchHandlerGroupModel> QueryHandlers { get; init; } = [];
}

/// <summary>
/// Model for a handler group (a class decorated with [CommandHandler] or [QueryHandler]).
/// </summary>
[PipelineModel]
public sealed record DispatchHandlerGroupModel
{
    /// <summary>
    /// The fully qualified name of the handler class.
    /// </summary>
    public required string HandlerType { get; init; }

    /// <summary>
    /// The handler class name without namespace.
    /// </summary>
    public required string HandlerTypeName { get; init; }

    /// <summary>
    /// The individual handler methods in this group.
    /// </summary>
    public required EquatableArray<DispatchHandlerMethodModel> Methods { get; init; }
}

/// <summary>
/// Model for a single dispatch handler method.
/// </summary>
[PipelineModel]
public sealed record DispatchHandlerMethodModel
{
    /// <summary>
    /// The method name (e.g., "Handle").
    /// </summary>
    public required string MethodName { get; init; }

    /// <summary>
    /// The fully qualified input type (command or query) — the first parameter.
    /// </summary>
    public required string InputType { get; init; }

    /// <summary>
    /// The input type name without namespace.
    /// </summary>
    public required string InputTypeName { get; init; }

    /// <summary>
    /// The fully qualified result type extracted from <c>Eff&lt;RT, T&gt;</c> return type.
    /// </summary>
    public required string ResultType { get; init; }

    /// <summary>
    /// The result type name without namespace.
    /// </summary>
    public required string ResultTypeName { get; init; }

    /// <summary>
    /// The fully qualified runtime type from <c>Eff&lt;RT, T&gt;</c>.
    /// </summary>
    public required string RuntimeType { get; init; }
}
