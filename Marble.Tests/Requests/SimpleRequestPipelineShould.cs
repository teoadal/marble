using Marble.Tests.Fakes.Requests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Marble.Tests.Requests
{
    public class SimpleRequestPipelineShould : BaseTest
    {
        private readonly IMediator _mediator;

        private readonly Mock<IRequestHandler<Request, RequestResponse>> _handler;

        public SimpleRequestPipelineShould()
        {
            _handler = MockHandler();

            _mediator = CreateRequestMediator<Request>(services => services
                .AddSingleton(_handler.Object));
        }

        [Fact]
        public void ProcessRequest()
        {
            SendRequest(_mediator);

            _handler.Verify(handler => handler
                .Handle(_request, default), Times.Once);
        }
        
        [Fact]
        public void ProcessRequestAsObject()
        {
            SendRequestAsObject(_mediator);

            _handler
                .Verify(handler => handler
                    .Handle(_request, default), Times.Once);
        }
    }
}