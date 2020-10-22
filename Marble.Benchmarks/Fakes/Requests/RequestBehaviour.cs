using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public sealed class RequestBehaviour : IPipelineBehavior<Request, RequestResponse>
    {
        public Task<RequestResponse> Handle(Request request, CancellationToken cancellationToken, RequestHandlerDelegate<RequestResponse> next)
        {
            request.Value++;
            return next();
        }
    }
}