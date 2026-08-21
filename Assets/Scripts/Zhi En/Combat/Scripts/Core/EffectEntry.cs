using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// one effect within an item or ability's effect list
    /// </summary>
    [System.Serializable]
    public struct EffectEntry
    {
        public EffectType effect;

        [Tooltip("damage/heal/buff amount for Damage/Heal Hp/Heal Cs/Buff Damage/Buff Defense. " +
                 "For Apply Status with status = Poison, this is instead the damage dealt per turn while poisoned. Unused otherwise.")]
        public int power;

        [Tooltip("only used when effect = Apply Status or Cure Status")]
        public StatusEffectType status;

        [Tooltip("only used when effect = Apply Status - how many of the target's own turns this status lasts")]
        public int duration;
    }
}
