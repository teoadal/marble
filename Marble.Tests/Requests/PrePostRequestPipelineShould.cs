using Marble.Tests.Fakes.Requests;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Marble.Tests.Requests
{
    public class PrePostRequestPipelineShould : BaseTest
    {
        private readonly IMediator _mediator;

        private readonly Mock<IRequestPreProcessor<Request>> _preProcessor;
        private readonly Mock<IRequestHandler<Request, RequestResponse>> _handler;
        private readonly Mock<IRequestPostProcessor<Request, RequestResponse>> _postProcessor;

        public PrePostRequestPipelineShould()
        {
            _preProcessor = MockPreProcessor();
            _handler = MockHandler();
            _postProcessor = MockPostProcessor();

            _mediator = CreateRequestMediator<Request>(services => services
                .AddSingleton(_preProcessor.Object)
                .AddSingleton(_handler.Object)
                .AddSingleton(_postProcessor.Object));
        }

        [Fact]
        public void PreProcessRequest()
        {
            SendRequest(_mediator);

            _preProcessor
                .Verify(processor => processor
                    .Process(_request, default), Times.Once);
        }

        [Fact]
        public void PreProcessMany()
        {
            var preProcessors = MockMany<IRequestPreProcessor<Request>>();

            var mediator = CreateRequestMediator<Request>(services =>
            {
                foreach (var preProcessor in preProcessors)
                {
                    services.AddSingleton(preProcessor.Object);
                }

                services
                    .AddSingleton(_handler.Object)
                    .AddSingleton(_postProcessor.Object);
            });

            SendRequest(mediator);

            foreach (var preProcessor in preProcessors)
            {
                preProcessor
                    .Verify(p => p
                        .Process(_request, default), Times.Once);
            }
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
                services
                    .AddSingleton(_preProcessor.Object)
                    .AddSingleton(_handler.Object);

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
    }
}