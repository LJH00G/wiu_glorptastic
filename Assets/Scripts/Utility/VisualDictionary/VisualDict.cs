using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.VisualDictionary
{
    [Serializable]
    public class VisualDict<K, V>
    {
        
        public Dictionary<K, V> dict = new();

        public V this[K key]
        {
            get => dict[key];
            set => dict[key] = value;
        }

        public int Count => dict.Count;

        public VisualDict() { }
        public VisualDict(VisualDict<K, V> other)
        {
            dict = new(other.dict);

#if UNITY_EDITOR
            serializableList.Clear();
            foreach (var entry in other.serializableList)
            {
                serializableList.Add(new(entry));
            }
#endif
        }


#if UNITY_EDITOR
        /// <summary>
        /// this only exists in editor, all uses of this should be within #if UNITY_EDITOR macro
        /// </summary>
        public List<DictEntry<K, V>> serializableList = new();

        /// <summary>
        /// set dict as what serializableList has
        /// </summary>
        public void OnValidate()
        {
            dict.Clear();
            for (int i = 0; i < serializableList.Count; i++)
            {
                var entry = serializableList[i];
                if (!dict.TryAdd(entry.key, entry.value))
                    Debug.LogError($"serializableList[{i}].key: {entry.key} repeated with one of the previous entries");
            }
        }

        /// <summary>
        /// set serializableList as what dict has
        /// </summary>
        public void InverseValidate()
        {
            serializableList.Clear();
            foreach (var entry in dict)
                serializableList.Add(new(entry.Key, entry.Value));
        }
#endif
    }
}

