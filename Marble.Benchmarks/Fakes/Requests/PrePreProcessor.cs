using System.Threading;
using System.Threading.Tasks;
using Marble.Attributes;
using MediatR.Pipeline;

namespace Marble.Benchmarks.Fakes.Requests
{
    [Ordering(1)]
    public class PrePreProcessor : IRequestPreProcessor<Request>
    {
        public Task Process(Request request, CancellationToken cancellationToken)
        {
            request.Value++;
            return TaskUtils.Completed;
        }
    }
}