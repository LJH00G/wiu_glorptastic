using System;

namespace Utility.VisualDictionary
{
    /// <summary>
    /// use a List<> to store this with types, this can be serialise and edited in the inspector, useful for making a dictionary
    /// </summary>
    [Serializable]
    public class DictEntry<K, V>
    {
        public K key;
        public V value;

        public DictEntry(K k, V v)
        {
            key = k;
            value = v;
        }
        public DictEntry(DictEntry<K, V> other)
        {
            key = other.key;
            value = other.value;
        }
    }
}

