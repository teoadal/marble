using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Marble.Attributes
{
    internal sealed class OrderingAttributeComparer : IComparer<Type>
    {
        private readonly int _defaultOrder;

        public OrderingAttributeComparer(int defaultOrder = int.MaxValue)
        {
            _defaultOrder = defaultOrder;
        }

        public int Compare(Type first, Type second)
        {
            var firstOrder = GetOrder(first);
            var secondOrder = GetOrder(second);

            return firstOrder.CompareTo(secondOrder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetOrder(Type type)
        {
            return OrderingAttribute.TryGetOrder(type, out var value)
                ? value
                : _defaultOrder;
        }
    }
}