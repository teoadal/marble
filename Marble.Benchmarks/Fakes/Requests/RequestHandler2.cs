using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public sealed class RequestHandler2 : IRequestHandler<Request2, int>
    {
        public Task<int> Handle(Request2 request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Environment.ProcessorCount + request.Value);
        }
    }
}