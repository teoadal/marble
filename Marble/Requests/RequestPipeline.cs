using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;

namespace Marble.Requests
{
    internal abstract class RequestPipeline<TRequest, TResponse> : IRequestPipeline<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public abstract Task<TResponse> Process(TRequest request, ServiceFactory serviceFactory,
            CancellationToken cancellationToken);

        public Task<TResponse> Process(IRequest<TResponse> request, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            return Process((TRequest) request, serviceFactory, cancellationToken);
        }

        protected static IRequestPreProcessor<TRequest>[] GetPreProcessors(ServiceFactory serviceFactory)
        {
            var processors = serviceFactory(typeof(IEnumerable<IRequestPreProcessor<TRequest>>));
            return (IRequestPreProcessor<TRequest>[]) processors;
        }

        protected static IRequestPostProcessor<TRequest, TResponse>[] GetPostProcessors(ServiceFactory serviceFactory)
        {
            var processors = serviceFactory(typeof(IEnumerable<IRequestPostProcessor<TRequest, TResponse>>));
            return (IRequestPostProcessor<TRequest, TResponse>[]) processors;
        }

        protected static Task<TResponse> Handle(TRequest request, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            var handler = serviceFactory(typeof(IRequestHandler<TRequest, TResponse>));
            return ((IRequestHandler<TRequest, TResponse>) handler).Handle(request, cancellationToken);
        }

        async Task<object?> IRequestPipeline.Process(object request, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            var response = await Process((TRequest) request, serviceFactory, cancellationToken);
            return response;
        }
    }
}