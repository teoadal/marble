using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;

namespace Marble.Benchmarks.Fakes.Requests
{
    public class PostProcessor2 : IRequestPostProcessor<Request2, int>
    {
        private readonly IMediator _mediator;

        public PostProcessor2(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Process(Request2 request, int response, CancellationToken cancellationToken)
        {
            var first = await _mediator.Send(new Request4(), cancellationToken);
            var second = await _mediator.Send(new Request5(), cancellationToken);

            request.Value += first + second;
        }
    }
}