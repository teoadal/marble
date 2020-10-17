using System.Threading.Tasks;
using FluentAssertions;
using Marble.Tests.Fakes.Requests;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Marble.Tests.Requests
{
    public class PostRequestPipelineShould : BaseTest
    {
        private readonly IMediator _mediator;

        private readonly Mock<IRequestHandler<Request, RequestResponse>> _handler;
        private readonly Mock<IRequestPostProcessor<Request, RequestResponse>> _postProcessor;

        public PostRequestPipelineShould()
        {
            _handler = MockHandler();
            _postProcessor = MockPostProcessor();

            _mediator = CreateRequestMediator<Request>(services => services
                .AddSingleton(_handler.Object)
                .AddSingleton(_postProcessor.Object));
        }

        [Fact]
        public void ProcessRequest()
        {
            SendRequest(_mediator);

            _handler
                .Verify(handler => handler
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
        
        [Fact]
        public void PostProcessRequest()
        {
            SendRequest(_mediator);

            _postProcessor
                .Verify(processor => processor
                    .Process(_request, _response, default), Times.Once);
        }

        [Fact]
        public void PostProcessMany()
        {
            var postProcessors = MockMany<IRequestPostProcessor<Request, RequestResponse>>();

            var mediator = CreateRequestMediator<Request>(services =>
            {
                services.AddSingleton(_handler.Object);

                foreach (var postProcessor in postProcessors)
                {
                    services.AddSingleton(postProcessor.Object);
                }
            });

            SendRequest(mediator);

            foreach (var postProcessor in postProcessors)
            {
                postProcessor
                    .Verify(p => p
                        .Process(_request, _response, default), Times.Once);
            }
        }

        [Fact]
        public async Task ReturnResult()
        {
            var response = await _mediator.Send(_request);
            response.Should().Be(_response);
        }
    }
}