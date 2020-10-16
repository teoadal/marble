using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Requests
{
    internal sealed class PreRequestPipeline<TRequest, TResponse> : RequestPipeline<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override async Task<TResponse> Process(TRequest request, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            foreach (var preProcessor in GetPreProcessors(serviceFactory))
            {
                await preProcessor.Process(request, cancellationToken);
            }

            return await Handle(request, serviceFactory, cancellationToken);
        }
    }
}