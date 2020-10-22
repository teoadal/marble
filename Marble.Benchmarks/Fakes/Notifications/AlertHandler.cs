using System.Threading;
using System.Threading.Tasks;
using Marble.Benchmarks.Fakes.Requests;
using MediatR;

namespace Marble.Benchmarks.Fakes.Notifications
{
    public sealed class AlertHandler : INotificationHandler<Alert>
    {
        private readonly IMediator _mediator;

        public AlertHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(Alert alert, CancellationToken cancellationToken)
        {
            var request3 = new Request3 {Value = alert.Value};
            await _mediator.Send(request3, cancellationToken);

            alert.Value = request3.Value;
        }
    }
}