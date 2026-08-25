using UnityEngine;


namespace Game.SO.Data.Item
{
    public abstract class ItemSO : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public Sprite Sprite { get; private set; }

        //readded description again
        [field: SerializeField, TextArea]
        public string Description { get; private set; }
    }
}