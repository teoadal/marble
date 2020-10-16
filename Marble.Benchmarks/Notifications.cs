using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Marble.Benchmarks.Fakes.Notifications;
using Marble.Bootstrap;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    public class Notifications
    {
        private IMediator _mediatr;
        private IMediator _marble;
        private Notification _notification;

        [GlobalSetup]
        public void Init()
        {
            _mediatr = new ServiceCollection()
                .AddMediatR(Assembly.GetExecutingAssembly())
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            _marble = new ServiceCollection()
                .AddMediator(mediator => mediator.RegisterPartsFromExecutingAssembly())
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            _notification = new Notification();
        }

        [Benchmark(Baseline = true)]
        public async Task<int> MediatR()
        {
            var sum = 0;
            for (var i = 0; i < 10; i++)
            {
                var task = _mediatr.Publish(_notification);
                await task;
                sum += task.Id;
            }

            return sum;
        }

        [Benchmark]
        public async Task<int> Marble()
        {
            var sum = 0;
            for (var i = 0; i < 10; i++)
            {
                var task = _marble.Publish(_notification);
                await task;
                sum += task.Id;
            }

            return sum;
        }
    }
}