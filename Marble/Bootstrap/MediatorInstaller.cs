using System;
using System.Reflection;
using Marble;
using MediatR;
using Marble.Bootstrap;
using Mediator = Marble.Mediator;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class MediatorInstaller
    {
        public static IServiceCollection AddMediator(this IServiceCollection services, Action<MediatorOptions> config)
        {
            var options = new MediatorOptions(services);
            config(options);

            if (services.Contains(typeof(IMediator)))
            {
                return services;
            }

            var mediatorLifetime = options.Lifetime;

            services
                .AddScoped<ServiceFactory>(provider => provider.GetService)
                .AddSingleton(new PipelineCollection())
                .Add(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), mediatorLifetime));

            services.Add(new ServiceDescriptor(typeof(ISender), ResolveMediator, mediatorLifetime));
            services.Add(new ServiceDescriptor(typeof(IPublisher), ResolveMediator, mediatorLifetime));

            return services;
        }

        public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
        {
            return AddMediator(services, options => options.RegisterParts(assemblies));
        }

        public static IServiceCollection AddMediator(this IServiceCollection services, params Type[] mediatorParts)
        {
            return AddMediator(services, options => options.RegisterParts(mediatorParts));
        }

        private static IMediator ResolveMediator(IServiceProvider provider)
        {
            return (IMediator) provider.GetService(typeof(IMediator));
        }
    }
}