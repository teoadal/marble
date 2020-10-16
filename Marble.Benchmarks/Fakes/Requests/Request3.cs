using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public class Request3 : IRequest
    {
        public int Value { get; set; }
    }
}