using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Notifications
{
    internal interface INotificationPipeline
    {
        Task Process(object notification, ServiceFactory serviceFactory, CancellationToken cancellationToken);
    }

    internal interface INotificationPipeline<in TNotification> : INotificationPipeline
        where TNotification : INotification
    {
        Task Process(TNotification notification, ServiceFactory serviceFactory, CancellationToken cancellationToken);
    }
}