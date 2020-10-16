using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Marble.Requests
{
    internal sealed partial class StandardRequestPipeline<TRequest, TResponse> : RequestPipeline<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private static BehavioursContext? _context;

        public override Task<TResponse> Process(TRequest request, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            var context = Interlocked.Exchange(ref _context, null);
            context ??= new BehavioursContext();

            var response = context.Process(request, serviceFactory, cancellationToken);

            context.Clear();
            Interlocked.Exchange(ref _context, context);

            return response;
        }

        private static async Task<TResponse> RunProcessors(TRequest request, ServiceFactory serviceFactory,
            CancellationToken cancellationToken)
        {
            foreach (var preProcessor in GetPreProcessors(serviceFactory))
            {
                await preProcessor.Process(request, cancellationToken);
            }

            var response = await Handle(request, serviceFactory, cancellationToken);

            foreach (var postProcessor in GetPostProcessors(serviceFactory))
            {
                await postProcessor.Process(request, response, cancellationToken);
            }

            return response;
        }
    }
}