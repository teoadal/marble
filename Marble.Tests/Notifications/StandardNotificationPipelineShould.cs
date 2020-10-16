using FluentAssertions;
using Marble.Bootstrap;
using Marble.Tests.Fakes.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Marble.Tests.Notifications
{
    public class StandardNotificationPipelineShould
    {
        private readonly Mock<INotificationHandler<Notification>> _handler;
        private readonly IMediator _mediator;
        private readonly Notification _notification;

        public StandardNotificationPipelineShould()
        {
            _handler = new Mock<INotificationHandler<Notification>>();

            _mediator = new ServiceCollection()
                .AddMediator(mediator => mediator.RegisterNotification<Notification>())
                .AddSingleton(_handler.Object)
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            _notification = new Notification();
        }

        [Fact]
        public void HandleNotification()
        {
            _mediator
                .Awaiting(mediator => mediator.Publish(_notification))
                .Should().NotThrow();

            _handler.Verify(handler => handler.Handle(_notification, default), Times.Once);
        }

        [Fact]
        public void HandleNotificationAsObject()
        {
            _mediator
                .Awaiting(mediator => mediator.Publish(_notification))
                .Should().NotThrow();

            _handler.Verify(handler => handler.Handle(_notification, default), Times.Once);

        }
    }
}