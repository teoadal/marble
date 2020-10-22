using System.Threading;
using System.Threading.Tasks;
using Marble.Attributes;
using MediatR.Pipeline;

namespace Marble.Tests.Fakes.Requests
{
    [Ordering(2)]
    public sealed class PreProcessor2 : IRequestPreProcessor<Request>
    {
        public Task Process(Request request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}