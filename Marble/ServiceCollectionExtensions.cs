using System;
using System.Collections.Generic;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Marble
{
    internal static class ServiceCollectionExtensions
    {
        public static bool Contains(this IServiceCollection services, Type serviceType)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var service in services)
            {
                if (service.ServiceType == serviceType) return true;
            }

            return false;
        }
        
        public static T[] GetArray<T>(this ServiceFactory factory)
        {
            return (T[]) factory(typeof(IEnumerable<T>));
        }

        public static bool TryDefineLifetime(this IServiceCollection services, Type implementation, out ServiceLifetime lifetime)
        {
            var result = (int) ServiceLifetime.Singleton;

            foreach (var constructor in implementation.GetConstructors())
            {
                if (!TryDefineLifetime(services, constructor, out var constructorLifetime))
                {
                    lifetime = default;
                    return false;
                }

                result = Math.Max(result, (int) constructorLifetime);
            }

            lifetime = (ServiceLifetime) result;
            return true;
        }

        public static bool TryDefineLifetime(this IServiceCollection services, ConstructorInfo constructor,
            out ServiceLifetime lifetime)
        {
            var result = (int) ServiceLifetime.Singleton;
            var parameters = constructor.GetParameters();
            foreach (var parameter in parameters)
            {
                if (!TryGetLifetime(services, parameter.ParameterType, out var parameterLifetime))
                {
                    lifetime = default;
                    return false;
                }

                result = Math.Max(result, (int) parameterLifetime);
            }

            lifetime = (ServiceLifetime) result;
            return true;
        }

        public static bool TryGetLifetime(this IServiceCollection services, Type serviceType,
            out ServiceLifetime lifetime)
        {
            foreach (var service in services)
            {
                if (service.ServiceType != serviceType) continue;

                lifetime = service.Lifetime;
                return true;
            }

            lifetime = default;
            return false;
        }
    }
}