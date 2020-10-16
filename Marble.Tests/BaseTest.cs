using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Marble.Bootstrap;
using Marble.Tests.Fakes.Requests;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Marble.Tests
{
    public abstract class BaseTest
    {
        // ReSharper disable InconsistentNaming
        protected readonly Request _request = new Request();

        protected readonly RequestResponse _response = new RequestResponse();
        // ReSharper restore InconsistentNaming

        protected static IMediator CreateRequestMediator<TRequest>(Action<IServiceCollection> configurator)
            where TRequest : IBaseRequest
        {
            var serviceCollection = new ServiceCollection();

            configurator(serviceCollection);

            return serviceCollection
                .AddMediator(options => options.RegisterRequest<TRequest>())
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();
        }

        protected Mock<T>[] MockMany<T>(Action<Mock<T>>? configurator = null, int count = 10)
            where T: class
        {
            var result = new List<Mock<T>>();
            for (var i = 0; i < count; i++)
            {
                var mock = new Mock<T>();
                configurator?.Invoke(mock);
                result.Add(mock);
            }

            return result.ToArray();
        }
        
        protected Mock<IPipelineBehavior<Request, RequestResponse>> MockPipelineBehaviour(Mock<IPipelineBehavior<Request, RequestResponse>>? behaviour = null)
        {
            behaviour ??= new Mock<IPipelineBehavior<Request, RequestResponse>>();

            behaviour
                .Setup(b => b.Handle(_request, default, It.IsNotNull<RequestHandlerDelegate<RequestResponse>>()))
                .Returns<Request, CancellationToken, RequestHandlerDelegate<RequestResponse>>((r, ct, next) => next());

            return behaviour;
        }

        protected Mock<IRequestHandler<Request, RequestResponse>> MockHandler()
        {
            var handler = new Mock<IRequestHandler<Request, RequestResponse>>();

            handler
                .Setup(h => h.Handle(_request, default))
                .ReturnsAsync(_response);

            return handler;
        }

        protected Mock<IRequestPostProcessor<Request, RequestResponse>> MockPostProcessor()
        {
            return new Mock<IRequestPostProcessor<Request, RequestResponse>>();
        }

        protected Mock<IRequestPreProcessor<Request>> MockPreProcessor()
        {
            return new Mock<IRequestPreProcessor<Request>>();
        }

        protected void SendRequest(IMediator mediator)
        {
            mediator
                .Awaiting(m => m.Send(_request))
                .Should().NotThrow();
        }
        
        protected void SendRequestAsObject(IMediator mediator)
        {
            mediator
                .Awaiting(m => m.Send((object)_request))
                .Should().NotThrow();
        }
    }
}