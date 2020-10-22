using System.Threading;
using System.Threading.Tasks;
using Marble.Attributes;
using MediatR;
using MediatR.Pipeline;

namespace Marble.Tests.Fakes.Requests
{
    [Ordering(1)]
    public sealed class PreProcessor1 : IRequestPreProcessor<Request>
    {
        // for testing DI
        // ReSharper disable UnusedParameter.Local
        public PreProcessor1(
                IMediator mediator,
                IPublisher publisher,
                ISender sender)
        // ReSharper restore UnusedParameter.Local
        {
        }

        public Task Process(Request request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}