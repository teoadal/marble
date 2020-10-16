using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Notifications
{
    internal sealed class ParallelNotificationPipeline<TNotification> : INotificationPipeline<TNotification>
        where TNotification : INotification
    {
        public Task Process(TNotification notification, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            var handlers = serviceFactory.GetArray<INotificationHandler<TNotification>>();
            return Task.WhenAll(handlers.Select(handler => handler.Handle(notification, cancellationToken)));
        }

        Task INotificationPipeline.Process(object notification, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            return Process((TNotification) notification, serviceFactory, cancellationToken);
        }
    }
}