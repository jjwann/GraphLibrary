using System;

namespace GraphLibrary
{
    /// <summary>
    /// Implementation of the max-binary heap data structure
    /// </summary>
    /// <typeparam name="TKey">The data type used to determine placement of an element on the heap</typeparam>
    /// <typeparam name="TValue">The value of an element in the heap</typeparam>
    public class MaxBinaryHeap<TKey, TValue> : BinaryHeap<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public MaxBinaryHeap()
            : base()
        {
        }

        public MaxBinaryHeap(int initialSize)
            : base(initialSize)
        {
        }

        protected override bool Compare(TKey firstValue, TKey secondValue)
        {
            return firstValue.CompareTo(secondValue) < 0;
        }
    }

    /// <summary>
    /// Implementation of the min-binary heap data structure
    /// </summary>
    /// <typeparam name="TKey">The data type used to determine placement of an element on the heap</typeparam>
    /// <typeparam name="TValue">The value of an element in the heap</typeparam>
    public class MinBinaryHeap<TKey, TValue> : BinaryHeap<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        public MinBinaryHeap()
            : base()
        {
        }

        public MinBinaryHeap(int initialSize)
            : base(initialSize)
        {
        }

        protected override bool Compare(TKey firstValue, TKey secondValue)
        {
            return firstValue.CompareTo(secondValue) > 0;
        }
    }
}
