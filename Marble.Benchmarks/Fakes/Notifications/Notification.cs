using MediatR;

namespace Marble.Benchmarks.Fakes.Notifications
{
    public class Notification : INotification
    {
        public int Value { get; set; }
    }
}