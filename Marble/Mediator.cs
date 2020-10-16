using System;
using System.Threading;
using System.Threading.Tasks;
using Marble.Notifications;
using Marble.Requests;
using MediatR;

namespace Marble
{
    internal sealed class Mediator : IMediator
    {
        private readonly PipelineCollection _pipelineCollection;
        private readonly ServiceFactory _serviceFactory;

        public Mediator(PipelineCollection pipelineCollection, ServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            _pipelineCollection = pipelineCollection;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            var pipeline = _pipelineCollection.GetRequestPipeline(request.GetType(), _serviceFactory);
            return ((IRequestPipeline<TResponse>) pipeline).Process(request, _serviceFactory, cancellationToken);
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken)
        {
            var pipeline = _pipelineCollection.GetRequestPipeline(request.GetType(), _serviceFactory);
            return pipeline.Process(request, _serviceFactory, cancellationToken);
        }

        public Task Publish(object notification, CancellationToken cancellationToken)
        {
            var notificationType = notification.GetType();

            if (!typeof(INotification).IsAssignableFrom(notificationType))
            {
                throw new InvalidOperationException($"Type {notificationType.Name} isn't implemented INotification");
            }

            var pipeline = _pipelineCollection.GetNotificationPipeline(notificationType, _serviceFactory);
            return pipeline.Process(notification, _serviceFactory, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken)
            where TNotification : INotification
        {
            var pipeline = _pipelineCollection.GetNotificationPipeline(typeof(TNotification), _serviceFactory);
            return ((INotificationPipeline<TNotification>) pipeline).Process(notification, _serviceFactory,
                cancellationToken);
        }
    }
}