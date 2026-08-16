using System;

namespace Utility.DictionaryEntry
{
    // to use this, inherit a derived entry with the types defined, then use a List<> to store the derived, this can be serialise and edited in the inspector, useful for making a dictionary
    [Serializable]
    public class DictEntry<K, V>
    {
        public K key;
        public V value;
    }
}

