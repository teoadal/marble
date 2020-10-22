using System;
using MediatR;

namespace Marble.Benchmarks.Fakes.Requests
{
    public sealed class RequestHandler5 : RequestHandler<Request5, int>
    {
        protected override int Handle(Request5 request)
        {
            return Environment.Version.Build;
        }
    }
}