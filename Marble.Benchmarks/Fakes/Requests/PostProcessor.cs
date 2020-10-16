using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;

namespace Marble.Benchmarks.Fakes.Requests
{
    public sealed class PostProcessor : IRequestPostProcessor<Request, RequestResponse>
    {
        public Task Process(Request request, RequestResponse response, CancellationToken cancellationToken)
        {
            request.Value++;
            return TaskUtils.Completed;
        }
    }
}