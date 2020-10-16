using System.Threading;
using System.Threading.Tasks;
using Marble.Attributes;
using MediatR.Pipeline;

namespace Marble.Tests.Fakes.Requests
{
    [Ordering(1)]
    public class PreProcessor1 : IRequestPreProcessor<Request>
    {
        public Task Process(Request request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}