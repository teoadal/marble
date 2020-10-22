using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marble.Attributes;
using Marble.Notifications;
using Marble.Requests;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Bootstrap
{
    internal sealed class MediatorRegistrar
    {
        private readonly Type _requestBehaviour;
        private readonly Type _requestPreProcessor;
        private readonly Type _requestHandler;
        private readonly Type _requestPostProcessor;

        private readonly Type[] _handlerTypes;

        public MediatorRegistrar()
        {
            _requestBehaviour = typeof(IPipelineBehavior<,>);
            _requestPreProcessor = typeof(IRequestPreProcessor<>);
            _requestHandler = typeof(IRequestHandler<,>);
            _requestPostProcessor = typeof(IRequestPostProcessor<,>);

            _handlerTypes = new[]
            {
                typeof(INotificationHandler<>),
                _requestBehaviour,
                _requestPreProcessor,
                _requestHandler,
                _requestPostProcessor
            };
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void RegisterNotification(Type notificationType, IServiceCollection services)
        {
            var pipelineDefinition = Attribute.IsDefined(notificationType, typeof(ParallelAttribute))
                ? typeof(ParallelNotificationPipeline<>)
                : typeof(StandardNotificationPipeline<>);

            var pipelineContract = typeof(INotificationPipeline<>).MakeGenericType(notificationType);
            var pipelineType = pipelineDefinition.MakeGenericType(notificationType);

            services.AddSingleton(pipelineContract, Activator.CreateInstance(pipelineType));
        }

        public void RegisterPipelines(Assembly[] assemblies, IServiceCollection services)
        {
            foreach (var assembly in assemblies.Distinct())
            {
                RegisterPipelines(assembly.DefinedTypes, services);
            }
        }

        public void RegisterPipelines(IEnumerable<Type> types, IServiceCollection services)
        {
            var mediatorParts = new List<MediatorPart>(100);

            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface || type.IsValueType) continue;

                foreach (var contract in type.GetInterfaces())
                {
                    if (!contract.IsGenericType) continue;

                    var definition = contract.GetGenericTypeDefinition();
                    if (Array.IndexOf(_handlerTypes, definition) == -1) continue;

                    mediatorParts.Add(new MediatorPart(type, contract, definition));
                }
            }
            // ReSharper restore LoopCanBeConvertedToQuery

            var partsComparer = new OrderingAttributeComparer();
            foreach (var effectGroup in mediatorParts.GroupBy(part => part.Effect))
            {
                var effectParts = effectGroup
                    .OrderBy(part => part.Type, partsComparer)
                    .ToArray();

                RegisterParts(effectParts, services);

                var effect = effectGroup.Key;
                if (typeof(INotification).IsAssignableFrom(effect))
                {
                    RegisterNotification(effect, services);
                }
                else
                {
                    var pipelineDefinition = DefinePipelineType(effect, effectParts.Select(part => part.Definition));
                    RegisterRequest(effect, services, pipelineDefinition);
                }
            }

            mediatorParts.Clear();
        }

        public void RegisterRequest(Type requestType, IServiceCollection services, Type? pipelineDefinition = null)
        {
            var responseType = requestType.GetResponseType();

            if (pipelineDefinition == null)
            {
                var partDefinitions = new List<Type>();
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var descriptor in services)
                {
                    var serviceType = descriptor.ServiceType;
                    if (!serviceType.IsGenericType || !serviceType.IsInterface) continue;

                    var definition = serviceType.GetGenericTypeDefinition();
                    if (Array.IndexOf(_handlerTypes, definition) == -1) continue;
                    if (serviceType.GetGenericArguments()[0] != requestType) continue;

                    partDefinitions.Add(definition);
                }

                pipelineDefinition = partDefinitions.Count == 0
                    ? typeof(StandardRequestPipeline<,>)
                    : DefinePipelineType(requestType, partDefinitions);
            }

            var pipelineContract = typeof(RequestPipeline<,>).MakeGenericType(requestType, responseType);
            var pipelineType = pipelineDefinition.MakeGenericType(requestType, responseType);

            services.AddSingleton(pipelineContract, Activator.CreateInstance(pipelineType));
        }

        private Type DefinePipelineType(Type requestType, IEnumerable<Type> partDefinitions)
        {
            bool hasBehaviour = false, hasPreProcessor = false, hasHandler = false, hasPostProcessor = false;

            foreach (var definition in partDefinitions)
            {
                if (definition == _requestBehaviour) hasBehaviour = true;
                else if (definition == _requestPreProcessor) hasPreProcessor = true;
                else if (definition == _requestHandler) hasHandler = true;
                else if (definition == _requestPostProcessor) hasPostProcessor = true;
            }

            Type pipelineDefinition;
            if (hasBehaviour) pipelineDefinition = typeof(StandardRequestPipeline<,>);
            else if (hasPreProcessor && hasPostProcessor) pipelineDefinition = typeof(PrePostRequestPipeline<,>);
            else if (hasPreProcessor) pipelineDefinition = typeof(PreRequestPipeline<,>);
            else if (hasPostProcessor) pipelineDefinition = typeof(PostRequestPipeline<,>);
            else pipelineDefinition = typeof(SimpleRequestPipeline<,>);

            if (!hasHandler) throw new HandlerNotFoundException(requestType);

            return pipelineDefinition;
        }

        private static void RegisterParts(MediatorPart[] parts, IServiceCollection services)
        {
            foreach (var part in parts)
            {
                var implementationType = part.Type;
                var serviceType = part.Contract;

                var lifetime = services.TryDefineLifetime(implementationType, out var definedLifetime)
                    ? definedLifetime
                    : ServiceLifetime.Transient;

                services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
            }
        }

        private readonly struct MediatorPart
        {
            public readonly Type Contract;
            public readonly Type Definition;
            public readonly Type Effect;
            public readonly Type Type;

            public MediatorPart(Type type, Type contract, Type definition)
            {
                Contract = contract;
                Definition = definition;
                Effect = contract.GetGenericArguments()[0];
                Type = type;
            }
        }
    }
}