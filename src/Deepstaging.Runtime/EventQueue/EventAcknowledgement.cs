// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.EventQueue;

/// <summary>
/// Represents the acknowledgement of an enqueued event, allowing callers
/// to await completion of all handlers.
/// </summary>
public sealed class EventAcknowledgement
{
    private readonly TaskCompletionSource _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    /// A task that completes when all handlers for the event have finished processing.
    /// </summary>
    public Task Completion => _completion.Task;

    /// <summary>
    /// Gets whether all handlers completed successfully.
    /// </summary>
    public bool IsCompletedSuccessfully => _completion.Task.IsCompletedSuccessfully;

    /// <summary>
    /// Gets the exception if any handler failed, or <c>null</c> if processing succeeded or is still in progress.
    /// </summary>
    public Exception? Exception => _completion.Task.IsFaulted ? _completion.Task.Exception?.InnerException : null;

    /// <summary>
    /// Marks the event as successfully processed by all handlers.
    /// </summary>
    internal void MarkCompleted() => _completion.TrySetResult();

    /// <summary>
    /// Marks the event as failed with the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    internal void MarkFailed(Exception exception) => _completion.TrySetException(exception);
}
