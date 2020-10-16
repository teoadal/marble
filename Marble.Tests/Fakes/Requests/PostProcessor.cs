using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;

namespace Marble.Tests.Fakes.Requests
{
    public sealed class PostProcessor : IRequestPostProcessor<Request, RequestResponse>
    {
        public Task Process(Request request, RequestResponse response, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}