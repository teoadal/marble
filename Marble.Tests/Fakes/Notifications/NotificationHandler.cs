using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Tests.Fakes.Notifications
{
    public class NotificationHandler : INotificationHandler<Notification>
    {
        public Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}