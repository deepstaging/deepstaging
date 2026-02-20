// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5

namespace Deepstaging.Projection.EventQueue;

using Attributes;
using Deepstaging.EventQueue;
using Models;

/// <summary>
/// Extension methods for building event queue models from symbols decorated with <see cref="EventQueueAttribute"/>.
/// </summary>
public static class EventQueueModelQueries
{
    extension(ValidSymbol<INamedTypeSymbol> container)
    {
        /// <summary>
        /// Queries the <see cref="EventQueueAttribute"/> on this type and builds the corresponding model.
        /// </summary>
        public EventQueueModel? QueryEventQueue()
        {
            var attributes = container.GetAttributes<EventQueueAttribute>();
            if (!attributes.Any()) return null;

            var attribute = attributes.First().AsQuery<EventQueueAttributeQuery>();

            return new EventQueueModel
            {
                QueueName = attribute.QueueName,
                ContainerName = container.Name,
                Namespace = container.Namespace ?? "Global",
                Accessibility = container.AccessibilityString,
                EventBaseType = attribute.EventBaseType?.GloballyQualifiedName,
                EventBaseTypeName = attribute.EventBaseType?.Name,
                Capacity = attribute.Capacity,
                MaxConcurrency = attribute.MaxConcurrency,
                TimeoutMilliseconds = attribute.TimeoutMilliseconds,
                SingleReader = attribute.SingleReader,
                SingleWriter = attribute.SingleWriter
            };
        }
    }

    extension(ValidSymbol<INamedTypeSymbol> handlerClass)
    {
        /// <summary>
        /// Queries this type for event handler methods and builds a handler group model.
        /// Handler methods must be static, accept a single event parameter, and return an Eff type.
        /// </summary>
        public EventQueueHandlerGroupModel? QueryEventQueueHandlerGroup()
        {
            var attributeData = handlerClass.Value.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass is { IsGenericType: true } cls &&
                    cls.OriginalDefinition.Name == "EventQueueHandlerAttribute");

            if (attributeData is null) return null;

            var query = new EventQueueHandlerAttributeQuery(attributeData);

            var methods = handlerClass
                .QueryMethods()
                .Where(m => m.IsStatic && m.Parameters.Length == 1)
                .Select(method => new EventHandlerMethodModel
                {
                    MethodName = method.Name,
                    EventType = method.Parameters[0].Type.GloballyQualifiedName,
                    EventTypeName = method.Parameters[0].Type.Name
                });

            return new EventQueueHandlerGroupModel
            {
                HandlerType = handlerClass.GloballyQualifiedName,
                HandlerTypeName = handlerClass.Name,
                RuntimeType = query.RuntimeType.GloballyQualifiedName,
                Methods = [..methods]
            };
        }
    }
}
