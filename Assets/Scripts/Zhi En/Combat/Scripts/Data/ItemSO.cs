using UnityEngine;

namespace Game.Combat
{
    [CreateAssetMenu(menuName = "Combat/Item", fileName = "New Item")]
    public class ItemSO : ScriptableObject
    {
        [Header("Display")]
        public string itemName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Effect")]
        public EffectType effect = EffectType.HEAL_HP;
        public int amount = 10;
        public CombatTargetType targetType = CombatTargetType.SELF;

        [Header("Behaviour")]
        [Tooltip("if true, one copy is removed from the inventory count on use")]
        public bool consumeOnUse = true;
    }

    /// <summary>a stack of an item + how many the player currently has. Lives inside PlayerLoadoutSO or an inventory manager.</summary>
    [System.Serializable]
    public struct ItemStack
    {
        public ItemSO item;
        public int count;
    }
}
