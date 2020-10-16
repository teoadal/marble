using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Marble.Bootstrap;
using Marble.Tests.Fakes.Notifications;
using Marble.Tests.Fakes.Requests;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Marble.Tests.Bootstrap
{
    public class MediatorInstallerShould
    {
        private readonly Assembly _currentAssembly;
        private readonly ServiceCollection _services;

        public MediatorInstallerShould()
        {
            _currentAssembly = typeof(MediatorInstallerShould).Assembly;
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddMediator()
        {
            _services.AddMediator(options => { });

            _services
                .Should().Contain(descriptor => descriptor.ServiceType == typeof(IMediator));
        }

        [Fact]
        public void AddServiceFactory()
        {
            _services.AddMediator(options => { });

            _services
                .Should().Contain(descriptor => descriptor
                    .ServiceType == typeof(ServiceFactory));
        }

        [Fact]
        public void RegisterConcretePart()
        {
            _services.AddMediator(mediator => mediator.RegisterParts(typeof(RequestHandler)));

            _services
                .Should().Contain(descriptor => descriptor
                    .ServiceType == typeof(IRequestHandler<Request, RequestResponse>));
        }

        [Fact]
        public void RegisterConcreteParts()
        {
            _services
                .AddMediator(mediator => mediator
                    .RegisterParts(typeof(RequestHandler), typeof(NotificationHandler)));

            _services.Select(descriptor => descriptor.ServiceType)
                .Should()
                .Contain(typeof(IRequestHandler<Request, RequestResponse>))
                .And
                .Contain(typeof(INotificationHandler<Notification>));
        }

        [Theory, MemberData(nameof(RequestPipelineParts))]
        public void RegisterMediatorWithAssembly(Type partType)
        {
            _services.AddMediator(_currentAssembly);

            _services.Select(descriptor => descriptor.ServiceType)
                .Should()
                .Contain(partType);
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void RegisterMediatorWithLifetime(ServiceLifetime lifetime)
        {
            IServiceCollection _ = lifetime switch
            {
                ServiceLifetime.Singleton => _services.AddMediator(mediator => mediator.AsSingleton()),
                ServiceLifetime.Scoped => _services.AddMediator(mediator => mediator.AsScoped()),
                ServiceLifetime.Transient => _services.AddMediator(mediator => mediator.AsTransient()),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            _services
                .First(descriptor => descriptor.ServiceType == typeof(IMediator)).Lifetime
                .Should().Be(lifetime);
        }

        [Theory, MemberData(nameof(RequestPipelineParts))]
        public void RegisterPartsFromAssembly(Type partType)
        {
            _services.AddMediator(mediator => mediator.RegisterParts(_currentAssembly));

            _services.Select(descriptor => descriptor.ServiceType)
                .Should()
                .Contain(partType);
        }
        
        [Fact]
        public void RegisterMediatorWithParts()
        {
            _services.AddMediator(typeof(RequestHandler), typeof(PreProcessor2));

            _services.Select(descriptor => descriptor.ServiceType)
                .Should()
                .Contain(new []
                {
                    typeof(IRequestHandler<Request, RequestResponse>),
                    typeof(IRequestPreProcessor<Request>)
                });
        }
        
        [Theory, MemberData(nameof(RequestPipelineParts))]
        public void RegisterPartsFromExecutingAssembly(Type partType)
        {
            _services.AddMediator(mediator => mediator.RegisterPartsFromExecutingAssembly());

            _services.Select(descriptor => descriptor.ServiceType)
                .Should()
                .Contain(partType);
        }
        
        [Fact]
        public void RegisterPartsInOrder()
        {
            _services.AddMediator(mediator => mediator.RegisterPartsFromExecutingAssembly());

            _services
                .Select(descriptor => descriptor.ImplementationType)
                .Should().ContainInOrder(typeof(PreProcessor1), typeof(PreProcessor2));
        }

        public static TheoryData<ServiceLifetime> Lifetimes = new TheoryData<ServiceLifetime>
        {
            ServiceLifetime.Singleton,
            ServiceLifetime.Scoped,
            ServiceLifetime.Transient
        };

        public static TheoryData<Type> RequestPipelineParts = new TheoryData<Type>
        {
            typeof(INotificationHandler<Notification>),
            typeof(IPipelineBehavior<Request, RequestResponse>),
            typeof(IRequestPreProcessor<Request>),
            typeof(IRequestHandler<Request, RequestResponse>),
            typeof(IRequestPostProcessor<Request, RequestResponse>)
        };
    }
}