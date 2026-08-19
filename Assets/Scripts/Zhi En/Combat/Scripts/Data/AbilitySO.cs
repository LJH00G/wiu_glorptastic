using UnityEngine;

namespace Game.Combat
{
    /// <summary>a curse special move, granted by the player's weapon/badges/etc.</summary>
    [CreateAssetMenu(menuName = "Combat/Ability", fileName = "New Ability")]
    public class AbilitySO : ScriptableObject
    {
        [Header("Display")]
        public string abilityName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Cost")]
        public int curseCost = 10;

        [Header("Effect")]
        public EffectType effect = EffectType.DAMAGE;
        public int power = 10;
        public CombatTargetType targetType = CombatTargetType.SINGLE_ENEMY;
    }
}
