using MediatR;

namespace Marble.Benchmarks.Fakes.Notifications
{
    public sealed class Alert : INotification
    {
        public int Value;
    }
}