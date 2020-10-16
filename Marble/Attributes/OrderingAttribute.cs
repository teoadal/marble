using System;
using System.Reflection;

namespace Marble.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OrderingAttribute : Attribute
    {
        public static bool TryGetOrder(Type type, out int value)
        {
            if (IsDefined(type, typeof(OrderingAttribute)))
            {
                var attribute = type.GetCustomAttribute<OrderingAttribute>();
                value = attribute.Value;
                return true;
            }

            value = default;
            return false;
        }

        public readonly int Value;

        public OrderingAttribute(int value)
        {
            Value = value;
        }
    }
}