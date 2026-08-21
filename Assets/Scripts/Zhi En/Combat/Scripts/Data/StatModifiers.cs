using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// base class for anything equippable that shifts the player's stats.
    /// Weapon / Armor / CursedGem / Accessory all derive from this so
    /// <see cref="PlayerLoadoutSO"/> can total them up generically.
    /// </summary>
    public abstract class StatModifierSO : ScriptableObject
    {
        [Header("Display")]
        public string itemName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Stats")]
        public int damageBonus;
        public int defenseBonus;
    }

    [CreateAssetMenu(menuName = "Combat/Equipment/Weapon", fileName = "New Weapon")]
    public class WeaponSO : StatModifierSO
    {
        // weapons primarily add damage, per design doc
    }

    [CreateAssetMenu(menuName = "Combat/Equipment/Armor", fileName = "New Armor")]
    public class ArmorSO : StatModifierSO
    {
        // armor primarily adds defense, per design doc
    }

    [CreateAssetMenu(menuName = "Combat/Equipment/Cursed Gem", fileName = "New Cursed Gem")]
    public class CursedGemSO : StatModifierSO
    {
        // this is the main levelling vector, upgrading the gem raises damageBonus/defenseBonus
        [Header("Gem Level (for your levelling system)")]
        public int level = 1;
    }

    [CreateAssetMenu(menuName = "Combat/Equipment/Accessory", fileName = "New Accessory")]
    public class AccessorySO : StatModifierSO
    {
        [Header("Accessory-only effects")]
        [Tooltip("multiplies the width of the green attack-mastery window, 1 = no change")]
        public float masteryWindowWidthMultiplier = 1f;
        [Tooltip("flat HP regenerated at the start of the player's turn")]
        public int hpRegenPerTurn = 0;
        [Tooltip("flat CS regenerated at the start of the player's turn")]
        public int csRegenPerTurn = 0;
    }
}
