using UnityEngine;


namespace Game.SO.Data.Item
{
    public abstract class ItemSO : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public Texture2D Texture { get; private set; }

        [field: SerializeField, TextArea]
        public string Description { get; private set; }

    }
}