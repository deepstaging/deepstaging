// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

using System.Diagnostics;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Deepstaging.Effects.Runtime;

/// <summary>
/// Extension methods for adding OpenTelemetry activity tracing to LanguageExt effects.
/// </summary>
public static class ActivityEffectExtensions
{
    /// <summary>
    /// Wraps an effect with OpenTelemetry activity tracing and optional structured logging.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method creates an <see cref="Activity"/> span around the effect execution,
    /// automatically recording duration, status, and any exceptions. When no
    /// <see cref="ActivityListener"/> is registered, activity creation is skipped with zero overhead.
    /// </para>
    /// <para>
    /// On success, the activity status is set to <see cref="ActivityStatusCode.Ok"/> and
    /// duration is logged at the specified level. On failure, the activity status is set to
    /// <see cref="ActivityStatusCode.Error"/> with exception details recorded as tags and events.
    /// </para>
    /// </remarks>
    /// <typeparam name="RT">The runtime type for the effect.</typeparam>
    /// <typeparam name="A">The result type produced by the effect.</typeparam>
    /// <param name="effect">The effect to instrument.</param>
    /// <param name="activityName">
    /// The name for the activity span (e.g., "AppRuntime.Email.SendAsync").
    /// </param>
    /// <param name="activitySource">
    /// The <see cref="ActivitySource"/> to create activities from. If <c>null</c>, no activity is created.
    /// </param>
    /// <param name="logger">
    /// Optional <see cref="ILogger"/> for structured logging. If <c>null</c>, no logging occurs.
    /// </param>
    /// <param name="logLevel">
    /// The log level for success messages. Errors are always logged at <see cref="LogLevel.Error"/>.
    /// </param>
    /// <param name="tags">Optional key-value pairs to add as activity tags.</param>
    /// <returns>A new effect that includes activity tracing around the original effect.</returns>
    public static Eff<RT, A> WithActivity<RT, A>(
        this Eff<RT, A> effect,
        string activityName,
        ActivitySource? activitySource = null,
        ILogger? logger = null,
        LogLevel logLevel = LogLevel.Information,
        IEnumerable<KeyValuePair<string, object?>>? tags = null)
    {
        return Eff<RT, A>.LiftIO(async rt =>
        {
            using var activity = activitySource?.StartActivity(activityName, ActivityKind.Internal);

            ApplyTags(activity, tags);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var value = await effect.RunAsync(rt);
                stopwatch.Stop();

                RecordSuccess(activity, stopwatch.ElapsedMilliseconds);
                LogSuccess(logger, logLevel, activityName, stopwatch.ElapsedMilliseconds);

                return value;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                RecordError(activity, ex, stopwatch.ElapsedMilliseconds);
                LogError(logger, ex, activityName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        });
    }

    private static void ApplyTags(Activity? activity, IEnumerable<KeyValuePair<string, object?>>? tags)
    {
        if (activity is null || tags is null)
            return;

        foreach (var tag in tags) activity.SetTag(tag.Key, tag.Value);
    }

    private static void RecordSuccess(Activity? activity, long durationMs)
    {
        if (activity is null)
            return;

        activity.SetStatus(ActivityStatusCode.Ok);
        activity.SetTag("duration_ms", durationMs);
    }

    private static void RecordError(Activity? activity, Exception ex, long durationMs)
    {
        if (activity is null)
            return;

        activity.SetStatus(ActivityStatusCode.Error, ex.Message);
        activity.SetTag("error", true);
        activity.SetTag("error.type", ex.GetType().Name);
        activity.SetTag("error.message", ex.Message);
        activity.SetTag("duration_ms", durationMs);

        var exceptionTags = new ActivityTagsCollection
        {
            { "exception.type", ex.GetType().FullName },
            { "exception.message", ex.Message },
            { "exception.stacktrace", ex.StackTrace }
        };
        activity.AddEvent(new ActivityEvent("exception", tags: exceptionTags));
    }

    private static void LogSuccess(ILogger? logger, LogLevel logLevel, string activityName, long durationMs)
    {
        if (logger?.IsEnabled(logLevel) != true)
            return;

        logger.Log(logLevel, "{ActivityName} completed in {DurationMs}ms", activityName, durationMs);
    }

    private static void LogError(ILogger? logger, Exception ex, string activityName, long durationMs)
    {
        logger?.LogError(ex, "{ActivityName} threw exception after {DurationMs}ms", activityName, durationMs);
    }
}