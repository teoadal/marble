using System.Linq;
using FluentAssertions;
using Marble.Tests.Fakes.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Marble.Tests.Notifications
{
    public class ParallelNotificationPipelineShould
    {
        private readonly Mock<INotificationHandler<ParallelNotification>>[] _handlers;
        private readonly IMediator _mediator;
        private readonly ParallelNotification _notification;

        public ParallelNotificationPipelineShould()
        {
            _handlers = Enumerable.Range(0, 10)
                .Select(_ => new Mock<INotificationHandler<ParallelNotification>>())
                .ToArray();

            var serviceCollection = new ServiceCollection();

            foreach (var handler in _handlers)
            {
                serviceCollection.AddSingleton(handler.Object);
            }

            _mediator = serviceCollection
                .AddMediator(mediator => mediator.RegisterNotification<ParallelNotification>())
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            _notification = new ParallelNotification();
        }

        [Fact]
        public void HandleNotification()
        {
            _mediator
                .Awaiting(mediator => mediator.Publish(_notification))
                .Should().NotThrow();

            foreach (var handler in _handlers)
            {
                handler
                    .Verify(h => h
                        .Handle(_notification, default), Times.Once);
            }
        }

        [Fact]
        public void HandleNotificationAsObject()
        {
            _mediator
                .Awaiting(mediator => mediator.Publish((object) _notification))
                .Should().NotThrow();

            foreach (var handler in _handlers)
            {
                handler
                    .Verify(h => h
                        .Handle(_notification, default), Times.Once);
            }
        }
    }
}