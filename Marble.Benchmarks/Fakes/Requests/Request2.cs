using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public class Request2 : IRequest<int>
    {
        public int Value { get; set; }
    }
}