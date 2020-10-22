using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Benchmarks.Fakes.Notifications
{
    public sealed class NotificationHandler : INotificationHandler<Notification>
    {
        public Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            notification.Value++;

            return TaskUtils.Completed;
        }
    }
}