using Game.SO.Data.Item;
using Game.SO.Data.Item.Sellable.Battle;
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
        [Header("Equipped Gear")]
        public CurseGemItemSO equippedGem;
        public WeaponItemSO equippedWeapon;
        public ArmourItemSO equippedArmor;
        [Tooltip("cap this in the inspector/UI to however many accessory slots you allow")]
        public List<AccessoryItemSO> equippedAccessories = new();

        [Header("Actions Menu")]
        public List<AbilitySO> knownAbilities = new();

        [Header("Items Menu")]
        // TODO: swap this for a proper persistent inventory system once one exists;
        // for now the battle reads/writes counts directly on this list.
        public List<Game.Inventory.ItemStack> inventory = new();

        public int TotalExtraDamage()
        {
            int total = 0;
            if (equippedGem) total += equippedGem.ExtraDamage;
            if (equippedWeapon) total += equippedWeapon.Dmage;
            foreach (var a in equippedAccessories)
                if (a) total += a.ExtraDamage;
            return total;
        }

        public int TotalExtraDefense()
        {
            int total = 0;
            if (equippedGem) total += equippedGem.ExtraDefence;
            if (equippedArmor) total += equippedArmor.Defence;
            foreach (var a in equippedAccessories)
                if (a) total += a.ExtraDefence;
            return total;
        }

        public float TotalMasteryWidthMultiplier()
        {
            float mult = 1f;
            foreach (var a in equippedAccessories)
                if (a) mult *= a.MasteryWindowWidthMultiplier;
            if (equippedGem) mult *= equippedGem.MasteryWindowWidthMultiplier;
            return mult;
        }

        public int TotalHPRegen()
        {
            int total = 0;
            foreach (var a in equippedAccessories)
                if (a) total += a.HPRegenPerTurn;
            if (equippedGem) total += equippedGem.HPRegenPerTurn;
            return total;
        }

        public int TotalCSRegen()
        {
            int total = 0;
            foreach (var a in equippedAccessories)
                if (a) total += a.CSRegenPerTurn;
            if (equippedGem) total += equippedGem.CSRegenPerTurn;
            return total;
        }
    }
}
