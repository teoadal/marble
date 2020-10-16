using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Requests
{
    internal sealed class SimpleRequestPipeline<TRequest, TResponse> : RequestPipeline<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override Task<TResponse> Process(TRequest request, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            return Handle(request, serviceFactory, cancellationToken);
        }
    }
}