using System;
using System.Collections.Generic;
using System.Threading;
using Marble.Notifications;
using Marble.Requests;
using MediatR;

namespace Marble
{
    internal sealed class PipelineCollection
    {
        private readonly Dictionary<Type, IRequestPipeline> _requests;
        private readonly Dictionary<Type, INotificationPipeline> _notifications;

        public PipelineCollection()
        {
            _requests = new Dictionary<Type, IRequestPipeline>(64);
            _notifications = new Dictionary<Type, INotificationPipeline>(64);
        }

        public IRequestPipeline GetRequestPipeline(Type requestType, ServiceFactory serviceFactory)
        {
            return GetOrAdd(
                _requests, serviceFactory, requestType,
                key => typeof(RequestPipeline<,>).MakeGenericType(key, key.GetResponseType()));
        }

        public INotificationPipeline GetNotificationPipeline(Type notificationType, ServiceFactory serviceFactory)
        {
            return GetOrAdd(
                _notifications, serviceFactory, notificationType,
                key => typeof(INotificationPipeline<>).MakeGenericType(key));
        }

        private static T GetOrAdd<T>(Dictionary<Type, T> collection,
            ServiceFactory serviceFactory,
            Type key, Func<Type, Type> builder)
        {
            if (collection.TryGetValue(key, out var pipeline)) return pipeline;

            var pipelineType = builder(key);
            var resolvedPipeline = (T) serviceFactory(pipelineType);

            if (resolvedPipeline == null)
            {
                throw new HandlerNotFoundException(key);
            }

            var lockTaken = false;
            Monitor.Enter(collection, ref lockTaken);

            collection[key] = resolvedPipeline;

            if (lockTaken) Monitor.Exit(collection);

            return resolvedPipeline;
        }
    }
}