using System.Threading;
using System.Threading.Tasks;
using Marble.Benchmarks.Fakes.Requests;
using MediatR;

namespace Marble.Benchmarks.Fakes.Notifications
{
    public class NotificationHandler : INotificationHandler<Notification>
    {
        private readonly IMediator _mediator;

        public NotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            var request3 = new Request3 {Value = notification.Value};
            await _mediator.Send(request3, cancellationToken);

            notification.Value = request3.Value;
        }
    }
}