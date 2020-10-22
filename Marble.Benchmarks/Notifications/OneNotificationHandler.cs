using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Marble.Benchmarks.Fakes.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Benchmarks.Notifications
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    public class OneNotificationHandler
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
            for (var i = 0; i < 10; i++)
            {
                await _mediatr.Publish(_notification);
            }

            return _notification.Value;
        }

        [Benchmark]
        public async Task<int> Marble()
        {
            for (var i = 0; i < 10; i++)
            {
                await _marble.Publish(_notification);
            }

            return _notification.Value;
        }
    }
}