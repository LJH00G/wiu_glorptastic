using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>a curse-meter-fuelled special move, granted by the player's weapon/badges/etc. (or used by an enemy via EnemyMove).</summary>
    [CreateAssetMenu(menuName = "Combat/Ability", fileName = "New Ability")]
    public class AbilitySO : ScriptableObject
    {
        [Header("Display")]
        public string abilityName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Cost")]
        public int curseCost = 10;

        [Header("Effects (all applied together, to the same target(s))")]
        public List<EffectEntry> effects = new();
        public CombatTargetType targetType = CombatTargetType.SINGLE_ENEMY;
    }
}