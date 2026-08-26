using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// one entry in an enemy's hardcoded move order.
    /// EnemyFSMController just walks down <see cref="EnemyDataSO.moveOrder"/> and repeats the last
    /// entry once it runs out, unless <see cref="EnemyDataSO.loopMoveOrder"/> is set, in which case it wraps to 0.
    /// </summary>
    [System.Serializable]
    public struct EnemyMove
    {
        public EnemyMoveType moveType;
        public CombatTargetType targetType;
        [Tooltip("only used when moveType == ABILITY")]
        public AbilitySO ability;
        [Tooltip("only used when moveType == ATTACK, multiplies the enemy's base damage stat, 1 = normal hit")]
        public float attackDamageMultiplier;
        [TextArea] public string flavourText;
    }

    /* TODO: this is a placeholder standing in for your teammate's enemy ScriptableObject.
    Once theirs exists, either point EnemyFSMController at their SO directly, or keep this
    as a thin wrapper/adapter with the same field names so nothing else needs to change. */
    [CreateAssetMenu(menuName = "Combat/Enemy Data (Placeholder)", fileName = "New Enemy")]
    public class EnemyDataSO : ScriptableObject
    {
        [Header("Display")]
        public string enemyName;
        public Sprite sprite; // raw shape for now, per your note

        [Header("Base Stats")]
        public int maxHP = 30;
        public int damage = 5;
        public int defense = 0;

        [Header("Move Order")]
        public EnemyMove[] moveOrder;
        public bool loopMoveOrder = true;

        [Header("Loot Table")]
        public LootTableSO enemyLootTable;
    }
}
