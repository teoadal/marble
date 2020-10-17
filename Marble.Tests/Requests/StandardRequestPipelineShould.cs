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
    public class StandardRequestPipelineShould : BaseTest
    {
        private readonly IMediator _mediator;

        private readonly Mock<IPipelineBehavior<Request, RequestResponse>> _behaviour;
        private readonly Mock<IRequestPreProcessor<Request>> _preProcessor;
        private readonly Mock<IRequestHandler<Request, RequestResponse>> _handler;
        private readonly Mock<IRequestPostProcessor<Request, RequestResponse>> _postProcessor;

        public StandardRequestPipelineShould()
        {
            _behaviour = MockPipelineBehaviour();
            _preProcessor = MockPreProcessor();
            _handler = MockHandler();
            _postProcessor = MockPostProcessor();

            _mediator = CreateRequestMediator<Request>(services => services
                .AddSingleton(_behaviour.Object)
                .AddSingleton(_preProcessor.Object)
                .AddSingleton(_handler.Object)
                .AddSingleton(_postProcessor.Object));
        }

        [Fact]
        public void ExecuteBehaviour()
        {
            SendRequest(_mediator);

            _behaviour
                .Verify(behavior => behavior
                    .Handle(_request, default, It.IsNotNull<RequestHandlerDelegate<RequestResponse>>()), Times.Once);
        }

        [Fact]
        public void ExecuteManyBehaviours()
        {
            var behaviours = MockMany<IPipelineBehavior<Request, RequestResponse>>(mock => MockPipelineBehaviour(mock));

            var mediator = CreateRequestMediator<Request>(services =>
            {
                foreach (var behaviour in behaviours)
                {
                    services.AddSingleton(behaviour.Object);
                }

                services
                    .AddSingleton(_preProcessor.Object)
                    .AddSingleton(_handler.Object)
                    .AddSingleton(_postProcessor.Object);
            });

            SendRequest(mediator);

            foreach (var behaviour in behaviours)
            {
                behaviour
                    .Verify(b => b
                        .Handle(
                            _request,
                            default,
                            It.IsNotNull<RequestHandlerDelegate<RequestResponse>>()), Times.Once);
            }
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
                    .AddSingleton(_behaviour.Object)
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

        [Fact]
        public async Task ReturnResult()
        {
            var response = await _mediator.Send(_request);
            response.Should().Be(_response);
        }
    }
}