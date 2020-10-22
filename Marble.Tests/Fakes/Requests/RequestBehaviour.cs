using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Tests.Fakes.Requests
{
    public sealed class RequestBehaviour : IPipelineBehavior<Request, RequestResponse>
    {
        public Task<RequestResponse> Handle(Request request, CancellationToken cancellationToken, RequestHandlerDelegate<RequestResponse> next)
        {
            return next();
        }
    }
}