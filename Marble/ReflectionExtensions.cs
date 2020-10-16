using System;
using MediatR;

namespace Marble
{
    internal static class ReflectionExtensions
    {
        public static Type GetResponseType(this Type requestType)
        {
            foreach (var requestInterface in requestType.GetInterfaces())
            {
                if (!requestInterface.IsGenericType) continue;
                if (requestInterface.GetGenericTypeDefinition() != typeof(IRequest<>)) continue;

                return requestInterface.GetGenericArguments()[0];
            }

            throw new InvalidOperationException($"Type {requestType.Name} isn't implemented IRequest");
        }
    }
}