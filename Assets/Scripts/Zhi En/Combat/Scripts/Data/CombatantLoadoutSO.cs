using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// persistent player loadout. This should live in a "GameData" folder and be the single
    /// source of truth the CombatManager reads from when a battle starts. Values here are
    /// placeholders - hook up your actual save/progression system to write into this SO
    /// (or to a runtime copy of it) later.
    /// </summary>
    [CreateAssetMenu(menuName = "Combat/Player Loadout", fileName = "PlayerLoadout")]
    public class PlayerLoadoutSO : ScriptableObject
    {
        [Header("Base Stats (design doc: tentatively 1 dmg, 0 def)")]
        public int baseDamage = 1;
        public int baseDefense = 0;
        public int maxHP = 64;
        public int maxCS = 30;

        [Header("Equipped Gear")]
        public CursedGemSO equippedGem;
        public WeaponSO equippedWeapon;
        public ArmorSO equippedArmor;
        [Tooltip("cap this in the inspector/UI to however many accessory slots you allow")]
        public List<AccessorySO> equippedAccessories = new();

        [Header("Actions Menu")]
        public List<AbilitySO> knownAbilities = new();

        [Header("Items Menu")]
        // TODO: swap this for a proper persistent inventory system once one exists;
        // for now the battle reads/writes counts directly on this list.
        public List<ItemStack> inventory = new();

        public int TotalDamage()
        {
            int total = baseDamage;
            if (equippedGem) total += equippedGem.damageBonus;
            if (equippedWeapon) total += equippedWeapon.damageBonus;
            if (equippedArmor) total += equippedArmor.damageBonus;
            foreach (var a in equippedAccessories)
                if (a) total += a.damageBonus;
            return total;
        }

        public int TotalDefense()
        {
            int total = baseDefense;
            if (equippedGem) total += equippedGem.defenseBonus;
            if (equippedWeapon) total += equippedWeapon.defenseBonus;
            if (equippedArmor) total += equippedArmor.defenseBonus;
            foreach (var a in equippedAccessories)
                if (a) total += a.defenseBonus;
            return total;
        }

        public float TotalMasteryWidthMultiplier()
        {
            float mult = 1f;
            foreach (var a in equippedAccessories)
                if (a) mult *= a.masteryWindowWidthMultiplier;
            return mult;
        }

        public int TotalHPRegen()
        {
            int total = 0;
            foreach (var a in equippedAccessories)
                if (a) total += a.hpRegenPerTurn;
            return total;
        }

        public int TotalCSRegen()
        {
            int total = 0;
            foreach (var a in equippedAccessories)
                if (a) total += a.csRegenPerTurn;
            return total;
        }
    }

    /// <summary>
    /// partner only gets Combat(Attack) + Actions
    /// </summary>
    [CreateAssetMenu(menuName = "Combat/Partner Loadout", fileName = "PartnerLoadout")]
    public class PartnerLoadoutSO : ScriptableObject
    {
        public string partnerName = "Partner";
        public int baseDamage = 1;
        public int baseDefense = 0;
        public int maxHP = 50;
        public int maxCS = 30;
        public List<AbilitySO> knownAbilities = new();

        public int TotalDamage() => baseDamage;
        public int TotalDefense() => baseDefense;
    }
}
