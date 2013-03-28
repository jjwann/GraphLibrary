using System;

namespace GraphLibrary
{
    /// <summary>
    /// Similar to KeyValuePair<TKey, TValue>, except that it is a reference type and contains an index property (to identify its place in a list)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class ReferencedKeyValuePair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public int Index { get; set; }

        public ReferencedKeyValuePair(TKey key, TValue value, int index)
        {
            Key = key;
            Value = value;
            Index = index;
        }
    }
}
