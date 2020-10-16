using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Marble.Benchmarks.Fakes.Requests;
using Marble.Bootstrap;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Benchmarks.Requests
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    public class OneHandler
    {
        private IMediator _mediatr;
        private IMediator _marble;
        private Request _request;

        [GlobalSetup]
        public void Init()
        {
            _mediatr = new ServiceCollection()
                .AddTransient<IRequestHandler<Request, RequestResponse>, RequestHandler>() // MediatR add as transient
                .AddMediatR(typeof(int).Assembly)
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            _marble = new ServiceCollection()
                .AddMediator(typeof(RequestHandler))
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            _request = new Request();
        }

        [Benchmark(Baseline = true)]
        public async Task<int> MediatR()
        {
            var sum = 0;
            for (var i = 0; i < 10; i++)
            {
                var response = await _mediatr.Send(_request);
                sum += response.Value;
            }

            return sum;
        }

        [Benchmark]
        public async Task<int> Marble()
        {
            var sum = 0;
            for (var i = 0; i < 10; i++)
            {
                var response = await _marble.Send(_request);
                sum += response.Value;
            }

            return sum;
        }
    }
}