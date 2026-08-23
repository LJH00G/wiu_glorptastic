
using System.Collections.Generic;
using UnityEngine;


namespace Game.Combat
{
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
        public int maxCS = 0;
        public List<AbilitySO> knownAbilities = new();

        public int TotalDamage() => baseDamage;
        public int TotalDefense() => baseDefense;
    }
}
