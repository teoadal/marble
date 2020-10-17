using System;
using System.Threading;
using System.Threading.Tasks;
using Marble.Attributes;
using MediatR;
using MediatR.Pipeline;

namespace Marble.Tests.Fakes.Requests
{
    [Ordering(1)]
    public class PreProcessor1 : IRequestPreProcessor<Request>
    {
        // for testing
        private readonly IMediator _mediator;
        private readonly IServiceProvider _provider;
        private readonly IPublisher _publisher;

        public PreProcessor1(IMediator mediator, IServiceProvider provider, IPublisher publisher)
        {
            _mediator = mediator;
            _provider = provider;
            _publisher = publisher;
        }

        public Task Process(Request request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}