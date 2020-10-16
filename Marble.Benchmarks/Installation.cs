using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Marble.Bootstrap;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    public class Installation
    {
        [Benchmark(Baseline = true)]
        public object MediatR()
        {
            return new ServiceCollection()
                .AddMediatR(Assembly.GetExecutingAssembly())
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();
        }

        [Benchmark]
        public object Marble()
        {
            return new ServiceCollection()
                .AddMediator(mediator => mediator.RegisterPartsFromExecutingAssembly())
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();
        }
    }
}