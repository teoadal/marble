using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public class Request : IRequest<RequestResponse>
    {
        public int Value { get; set; }
    }
}