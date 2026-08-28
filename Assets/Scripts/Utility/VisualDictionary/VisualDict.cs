using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Utility.VisualizableDictionary
{
    [Serializable]
    public class VisualizableDict<K, V>
    {
        
        public Dictionary<K, V> dict = new();

        public V this[K key]
        {
            get
            {
                if (dict.Count == 0)
                    OnValidate();

                return dict[key];
            }
            set
            {
                if (dict.Count == 0)
                    OnValidate();

                dict[key] = value;
            }
        }

        public int Count => dict.Count;

        public VisualizableDict() { }
        public VisualizableDict(VisualizableDict<K, V> other)
        {
            dict = new(other.dict);

            serializableList.Clear();
            foreach (var entry in other.serializableList)
            {
                serializableList.Add(new(entry));
            }
        }


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
    }
}

