using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public sealed class Request : IRequest<RequestResponse>
    {
        public int Value;
    }
}