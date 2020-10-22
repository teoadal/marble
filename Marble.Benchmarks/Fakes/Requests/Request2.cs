using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public sealed class Request2 : IRequest<int>
    {
        public int Value;
    }
}