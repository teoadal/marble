using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Requests
{
    internal sealed class PostRequestPipeline<TRequest, TResponse> : RequestPipeline<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override async Task<TResponse> Process(TRequest request, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            var response = await Handle(request, serviceFactory, cancellationToken);

            foreach (var postProcessor in GetPostProcessors(serviceFactory))
            {
                await postProcessor.Process(request, response, cancellationToken);
            }

            return response;
        }
    }
}