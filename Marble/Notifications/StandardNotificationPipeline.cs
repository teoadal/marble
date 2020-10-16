using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Notifications
{
    internal sealed class StandardNotificationPipeline<TNotification> : INotificationPipeline<TNotification>
        where TNotification : INotification
    {
        public async Task Process(TNotification notification, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            var handlers = serviceFactory.GetArray<INotificationHandler<TNotification>>();
            foreach (var handler in handlers)
            {
                await handler.Handle(notification, cancellationToken);
            }
        }

        Task INotificationPipeline.Process(object notification, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            return Process((TNotification) notification, serviceFactory, cancellationToken);
        }
    }
}