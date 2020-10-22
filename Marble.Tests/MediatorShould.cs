using System;
using System.Threading.Tasks;
using FluentAssertions;
using Marble.Tests.Fakes.Notifications;
using Marble.Tests.Fakes.Requests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Marble.Tests
{
    public class MediatorShould
    {
        private readonly IMediator _mediator;
        private readonly Request _request;

        public MediatorShould()
        {
            _mediator = new ServiceCollection()
                .AddMediator(options => options.RegisterPartsFromExecutingAssembly())
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            _request = new Request();
        }

        [Fact]
        public async Task GetRequestResponse()
        {
            var response = await _mediator.Send(_request);
            response.Should().NotBeNull();
        }

        [Fact]
        public async Task GetObjectRequestResponse()
        {
            var response = await _mediator.Send((object) _request);
            response
                .Should().NotBeNull()
                .And.BeOfType<RequestResponse>();
        }

        [Fact]
        public void PublishNotification()
        {
            _mediator
                .Awaiting(mediator => mediator.Publish(new Notification()))
                .Should().NotThrow();
        }

        [Fact]
        public void PublishNotificationManyTimes()
        {
            for (int i = 0; i < 10; i++)
            {
                _mediator
                    .Awaiting(mediator => mediator.Publish(new Notification()))
                    .Should().NotThrow();
            }
        }
        
        [Fact]
        public void PublishNotificationAsObject()
        {
            _mediator
                .Awaiting(mediator => mediator.Publish((object) new Notification()))
                .Should().NotThrow();
        }

        [Fact]
        public void SendRequest()
        {
            _mediator
                .Awaiting(mediator => mediator.Send(_request))
                .Should().NotThrow();
        }

        [Fact]
        public void SendRequestManyTimes()
        {
            for (int i = 0; i < 10; i++)
            {
                _mediator
                    .Awaiting(mediator => mediator.Send(_request))
                    .Should().NotThrow();
            }
        }
        
        [Fact]
        public void SendRequestAsObject()
        {
            _mediator
                .Awaiting(mediator => mediator.Send((object) _request))
                .Should().NotThrow();
        }

        [Fact]
        public void ThrowIfRequestHandlerNotFound()
        {
            _mediator
                .Awaiting(mediator => mediator.Send(new NotRegisteredRequest()))
                .Should().Throw<HandlerNotFoundException>();
        }

        [Fact]
        public void ThrowIfRequestNotImplementedInterface()
        {
            _mediator
                .Awaiting(mediator => mediator.Send(new object()))
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowIfNotificationHandlerNotFound()
        {
            _mediator
                .Awaiting(mediator => mediator.Publish(new NotRegisteredNotification()))
                .Should().Throw<HandlerNotFoundException>();
        }

        [Fact]
        public void ThrowIfNotificationNotImplementedInterface()
        {
            _mediator
                .Awaiting(mediator => mediator.Publish(new object()))
                .Should().Throw<ArgumentException>();
        }
    }
}