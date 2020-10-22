using System;
using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public sealed class RequestHandler4 : RequestHandler<Request4, int>
    {
        protected override int Handle(Request4 request)
        {
            return Environment.CurrentManagedThreadId;
        }
    }
}