using System;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Bootstrap
{
    public sealed class MediatorOptions
    {
        internal ServiceLifetime Lifetime { get; private set; }

        public readonly IServiceCollection Services;

        private readonly MediatorRegistrar _registrar;

        public MediatorOptions(IServiceCollection services)
        {
            Lifetime = ServiceLifetime.Scoped;
            Services = services;
            _registrar = new MediatorRegistrar();
        }

        public MediatorOptions RegisterNotification<TNotification>() where TNotification : INotification
        {
            _registrar.RegisterNotification(typeof(TNotification), Services);
            return this;
        }

        public MediatorOptions RegisterParts(params Type[] mediatorParts)
        {
            _registrar.RegisterPipelines(mediatorParts, Services);
            return this;
        }

        public MediatorOptions RegisterParts(params Assembly[] assemblies)
        {
            _registrar.RegisterPipelines(assemblies, Services);
            return this;
        }

        public MediatorOptions RegisterPartsFromExecutingAssembly()
        {
            _registrar.RegisterPipelines(new[] {Assembly.GetCallingAssembly()}, Services);
            return this;
        }

        public MediatorOptions RegisterRequest<TRequest>() where TRequest : IBaseRequest
        {
            _registrar.RegisterRequest(typeof(TRequest), Services);
            return this;
        }

        public MediatorOptions AsSingleton()
        {
            Lifetime = ServiceLifetime.Singleton;
            return this;
        }

        public MediatorOptions AsScoped()
        {
            Lifetime = ServiceLifetime.Scoped;
            return this;
        }

        public MediatorOptions AsTransient()
        {
            Lifetime = ServiceLifetime.Transient;
            return this;
        }
    }
}