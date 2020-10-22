using System.Threading;
using System.Threading.Tasks;
using Marble.Attributes;
using Marble.Benchmarks.Fakes.Notifications;
using MediatR;
using MediatR.Pipeline;

namespace Marble.Benchmarks.Fakes.Requests
{
    [Ordering(2)]
    public sealed class PreProcessor : IRequestPreProcessor<Request>
    {
        private readonly IMediator _mediator;

        public PreProcessor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Process(Request request, CancellationToken cancellationToken)
        {
            var request2 = new Request2();
            var response = await _mediator.Send(request2, cancellationToken);

            var notification = new Alert {Value = request.Value};
            await _mediator.Publish(notification, cancellationToken);

            request.Value += (response + request2.Value) / notification.Value;
        }
    }
}