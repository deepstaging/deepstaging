// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Generators.Writers.EventQueue;

using Deepstaging.Projection.EventQueue.Models;

/// <summary>
/// Emits the EventQueue effect module with Enqueue, EnqueueWithAck, and EnqueueAndWait methods.
/// </summary>
public static class EventQueueEffectModuleWriter
{
    extension(EventQueueModel model)
    {
        /// <summary>
        /// Emits the event queue effect module as a static partial class.
        /// </summary>
        public OptionalEmit WriteEffectModule()
        {
            var channelType = model.EventBaseType is not null
                ? $"global::Deepstaging.EventQueue.EventQueueChannel<{model.EventBaseType}>"
                : "global::Deepstaging.EventQueue.EventQueueChannel<object>";

            var eventType = model.EventBaseType ?? "object";

            return TypeBuilder
                .Parse($"{model.Accessibility} static partial class {model.ContainerName}")
                .InNamespace(model.Namespace)
                .AddUsings("LanguageExt", "LanguageExt.Effects")
                .AddUsings("Deepstaging.EventQueue")
                .AddField(FieldBuilder
                    .Parse($"private static {channelType}? _channel")
                )
                .AddMethod(MethodBuilder
                    .Parse($"internal static void Initialize({channelType} channel)")
                    .WithBody(b => b
                        .AddStatement("_channel = channel")
                    )
                )
                .AddMethod(MethodBuilder
                    .Parse($"public static Eff<RT, Unit> Enqueue<RT>({eventType} @event)")
                    .WithExpressionBody("Eff<RT, Unit>.LiftIO(_ => { _channel!.Enqueue(@event); return default; })")
                )
                .AddMethod(MethodBuilder
                    .Parse($"public static Eff<RT, EventAcknowledgement> EnqueueWithAck<RT>({eventType} @event)")
                    .WithExpressionBody("Eff<RT, EventAcknowledgement>.LiftIO(_ => _channel!.EnqueueWithAck(@event))")
                )
                .AddMethod(MethodBuilder
                    .Parse($"public static Eff<RT, Unit> EnqueueAndWait<RT>({eventType} @event)")
                    .WithExpressionBody("Eff<RT, Unit>.LiftIO(async _ => { await _channel!.EnqueueAndWait(@event); return default; })")
                )
                .WithXmlDoc(xml => xml
                    .WithSummary($"Event queue effect module for the '{model.QueueName}' queue.")
                )
                .Emit();
        }
    }
}
