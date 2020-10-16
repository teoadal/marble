using System;
using MediatR;

namespace Marble
{
    public sealed class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(Type effectType)
            : base(typeof(INotification).IsAssignableFrom(effectType)
                ? $"Handler for notification {effectType.Name} isn't found"
                : $"Handler for request {effectType.Name} isn't found")
        {
        }
    }
}