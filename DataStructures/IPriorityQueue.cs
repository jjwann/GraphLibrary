using System.Collections.Generic;

namespace GraphLibrary
{
    /// <summary>
    /// Defines the available functionality for a priority queue abstract data type
    /// </summary>
    /// <typeparam name="TKey">The data type used to determine order of elements</typeparam>
    /// <typeparam name="TValue">The data type of the stored value of each element</typeparam>
    public interface IPriorityQueue<TKey, TValue>
    {
        /// <summary>
        /// Inserts a new element to the priority queue, ordered by key
        /// </summary>
        /// <param name="key">Used to determine the placement of the new element</param>
        /// <param name="value">The value of the new element</param>
        /// <returns>true if the insert is successful; false otherwise</returns>
        bool Insert(TKey key, TValue value);

        /// <summary>
        /// Returns a copy of the first element without removing it from the priority queue
        /// </summary>
        /// <param name="extractedValue">Contains the key and value of the first element</param>
        /// <returns>true if there are any elements in the queue; false otherwise</returns>
        bool Peek(out KeyValuePair<TKey, TValue> extractedValue);

        /// <summary>
        /// Removes the first element of the property queue
        /// </summary>
        /// <param name="extractedValue">Contains the key and value of the first element</param>
        /// <returns>true if there are any elements in the queue; false otherwise</returns>
        bool Extract(out KeyValuePair<TKey, TValue> extractedValue);

        /// <summary>
        /// Modify the key of the specified element if it would cause it to move closer to the front of the priority queue. Then, move the element to the right place in the order.
        /// If modifying the key won't engender moving the element closer to the front, do nothing.
        /// </summary>
        /// <param name="newKeyValue">The new key value of the element</param>
        /// <param name="itemValue">The value of the element of which to change the key</param>
        /// <returns>true if the key value changed; false otherwise</returns>
        bool ChangeKeyAndMoveUp(TKey newKeyValue, TValue itemValue);
    }
}
