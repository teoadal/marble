using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public sealed class RequestHandler3 : AsyncRequestHandler<Request3>
    {
        protected override Task Handle(Request3 request, CancellationToken cancellationToken)
        {
            request.Value += Environment.TickCount;
            return TaskUtils.Completed;
        }
    }
}