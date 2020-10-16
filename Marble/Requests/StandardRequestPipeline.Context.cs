using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Requests
{
    internal sealed partial class StandardRequestPipeline<TRequest, TResponse>
    {
        private sealed class BehavioursContext
        {
            private readonly RequestHandlerDelegate<TResponse> _next;

            private IEnumerator<IPipelineBehavior<TRequest, TResponse>> _behaviours = null!;
            private CancellationToken _cancellationToken;
            private TRequest _request = default!;
            private ServiceFactory _serviceFactory = null!;

            public BehavioursContext()
            {
                _next = Next;
            }

            public Task<TResponse> Process(TRequest request, ServiceFactory serviceFactory,
                CancellationToken cancellationToken)
            {
                _behaviours = serviceFactory.GetInstances<IPipelineBehavior<TRequest, TResponse>>().GetEnumerator();
                _cancellationToken = cancellationToken;
                _request = request;
                _serviceFactory = serviceFactory;

                return Next();
            }

            public void Clear()
            {
                _behaviours.Dispose();
                _behaviours = null!;
                _cancellationToken = default;
                _request = default!;
                _serviceFactory = null!;
            }

            private Task<TResponse> Next()
            {
                return _behaviours.MoveNext()
                    ? _behaviours.Current!.Handle(_request, _cancellationToken, _next)
                    : RunProcessors(_request, _serviceFactory, _cancellationToken);
            }
        }
    }
}