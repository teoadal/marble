using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Requests
{
    internal interface IRequestPipeline
    {
        Task<object?> Process(object request, ServiceFactory serviceFactory, CancellationToken cancellationToken);
    }

    internal interface IRequestPipeline<TResponse> : IRequestPipeline
    {
        Task<TResponse> Process(IRequest<TResponse> request, ServiceFactory serviceFactory,
            CancellationToken cancellationToken);
    }
}