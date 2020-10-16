using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public class RequestHandler : IRequestHandler<Request, RequestResponse>
    {
        public Task<RequestResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RequestResponse {Value = Environment.TickCount + request.Value});
        }
    }
}