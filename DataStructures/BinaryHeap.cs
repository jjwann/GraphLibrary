using System.Collections.Generic;

namespace GraphLibrary
{
    /// <summary>
    /// Implementation of the binary heap data structure
    /// </summary>
    /// <typeparam name="TKey">The data type used to determine placement of an element on the heap</typeparam>
    /// <typeparam name="TValue">The value of an element in the heap</typeparam>
    public abstract class BinaryHeap<TKey, TValue> : IPriorityQueue<TKey, TValue>
    {
        /// <summary>
        /// Where the elements are ultimately accessed
        /// </summary>
        private List<ReferencedKeyValuePair<TKey, TValue>> backingArray;

        /// <summary>
        /// Enables lookup of an element's index in the backing array
        /// </summary>
        private Dictionary<TValue, ReferencedKeyValuePair<TKey, TValue>> itemDictionary;

        protected BinaryHeap()
        {
            backingArray = new List<ReferencedKeyValuePair<TKey, TValue>>();
            itemDictionary = new Dictionary<TValue, ReferencedKeyValuePair<TKey, TValue>>();
        }

        protected BinaryHeap(int initialSize)
        {
            backingArray = new List<ReferencedKeyValuePair<TKey, TValue>>(initialSize);
            itemDictionary = new Dictionary<TValue, ReferencedKeyValuePair<TKey, TValue>>(initialSize);
        }

        /// <summary>
        /// Compares two keys
        /// </summary>
        /// <param name="firstValue">The first key</param>
        /// <param name="secondValue">The second key</param>
        /// <returns>true if the first key is such that the corresponding element would be below the element corresponding the second key in the heap; false otherwise</returns>
        protected abstract bool Compare(TKey firstValue, TKey secondValue);

        public bool Insert(TKey key, TValue value)
        {
            bool isSuccess = false;

            if (key != null && value != null && !itemDictionary.ContainsKey(value))
            {
                isSuccess = true;

                int childIndex = backingArray.Count;
                ReferencedKeyValuePair<TKey, TValue> entry = new ReferencedKeyValuePair<TKey, TValue>(key, value, childIndex);

                backingArray.Add(entry);
                itemDictionary.Add(value, entry);

                BubbleUp(childIndex);
            }

            return isSuccess;
        }

        public bool Peek(out KeyValuePair<TKey, TValue> extractedValue)
        {
            bool foundValue = false;

            if (backingArray.Count == 0)
            {
                extractedValue = new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
            }
            else
            {
                foundValue = true;
                ReferencedKeyValuePair<TKey, TValue> data = backingArray[0];
                extractedValue = new KeyValuePair<TKey, TValue>(data.Key, data.Value);
            }

            return foundValue;
        }

        public bool Extract(out KeyValuePair<TKey, TValue> extractedValue)
        {
            if (Peek(out extractedValue))
            {
                itemDictionary.Remove(backingArray[0].Value);

                int removeIndex = backingArray.Count - 1;

                backingArray[0] = backingArray[removeIndex];
                backingArray.RemoveAt(removeIndex);

                if (backingArray.Count > 0)
                {
                    backingArray[0].Index = 0;

                    int parentIndex = 0;
                    BubbleDown(parentIndex);
                }

                return true;
            }

            return false;
        }

        public bool ChangeKeyAndMoveUp(TKey newKeyValue, TValue itemValue)
        {
            bool isReplaced = false;
            ReferencedKeyValuePair<TKey, TValue> entry;

            if (newKeyValue != null && itemValue != null && itemDictionary.TryGetValue(itemValue, out entry))
            {
                if (Compare(entry.Key, newKeyValue))
                {
                    isReplaced = true;

                    entry.Key = newKeyValue;
                    BubbleUp(entry.Index);
                }
            }

            return isReplaced;
        }

        /// <summary>
        /// If necessary, moves the element referenced by the given index up the heap until it is in its correct place in the heap
        /// </summary>
        /// <param name="childIndex">The index of the element</param>
        private void BubbleUp(int childIndex)
        {
            int parentIndex = (childIndex - 1) >> 1;

            if (parentIndex > -1)
            {
                while (childIndex > 0 && Compare(backingArray[parentIndex].Key, backingArray[childIndex].Key))
                {
                    Swap(childIndex, parentIndex);

                    childIndex = parentIndex;
                    parentIndex = (childIndex - 1) >> 1;
                }
            }
        }

        /// <summary>
        /// If necessary, moves the element referenced by the given index down the heap until it is in its correct place in the heap
        /// </summary>
        /// <param name="childIndex">The index of the element</param>
        private void BubbleDown(int parentIndex)
        {
            int firstChildIndex = (parentIndex << 1) | 0x1;
            int secondChildIndex = firstChildIndex + 1;

            int swapIndex = 0;
            bool mustSwap;

            do
            {
                mustSwap = false;

                if (firstChildIndex < backingArray.Count && Compare(backingArray[parentIndex].Key, backingArray[firstChildIndex].Key))
                {
                    swapIndex = firstChildIndex;
                    mustSwap = true;
                }

                if (secondChildIndex < backingArray.Count && Compare(backingArray[parentIndex].Key, backingArray[secondChildIndex].Key))
                {
                    if (Compare(backingArray[firstChildIndex].Key, backingArray[secondChildIndex].Key))
                    {
                        swapIndex = secondChildIndex;
                    }

                    mustSwap = true;
                }

                if (mustSwap)
                {
                    Swap(swapIndex, parentIndex);

                    parentIndex = swapIndex;
                    firstChildIndex = (parentIndex << 1) | 0x1;
                    secondChildIndex = firstChildIndex + 1;
                }
            } while (mustSwap);
        }

        /// <summary>
        /// Swaps two elements in the backing array
        /// </summary>
        /// <param name="firstIndex">index of the first element to swap</param>
        /// <param name="secondIndex">index of the second element to swap</param>
        private void Swap(int firstIndex, int secondIndex)
        {
            ReferencedKeyValuePair<TKey, TValue> temp = backingArray[firstIndex];

            backingArray[firstIndex] = backingArray[secondIndex];
            backingArray[secondIndex] = temp;

            backingArray[firstIndex].Index = firstIndex;
            backingArray[secondIndex].Index = secondIndex;
        }
    }
}
