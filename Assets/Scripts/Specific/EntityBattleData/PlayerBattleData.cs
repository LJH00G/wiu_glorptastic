using Game.Inventory;
using Game.SO.Data.Item;
using UnityEngine;

namespace Game.Combat.Integration
{
    public class PlayerBattleData
    {
        [field: SerializeField]
        public int MaxHP { get; private set; }
        [field: SerializeField]
        public int MaxCurse { get; private set; }
        /// shimi shimi
        //[field: SerializeField, DisplayOnly]
        //public string LastCheckpointID { get; private set; } = "";

        //[field: SerializeField, DisplayOnly]
        //public string LastSceneName { get; private set; } = "";
        [SerializeField]
        int currentHP;
        public int CurrentHP
        {
            get => currentHP;
            set
            {
                currentHP = value;
                if (currentHP > MaxHP)
                    currentHP = MaxHP;
            }
        }
        [SerializeField]
        int currentCurse;
        public int CurrentCurse
        {
            get => currentCurse;
            set
            {
                currentCurse = value;
                if (currentCurse > MaxCurse)
                    currentCurse = MaxCurse;
            }
        }


        public void Refresh()
        {
            var accessories = InventoryManager.GetEquipedAccessories();
            var gem = InventoryManager.TryGetItemInList(out CurseGemItemSO gemItem) ? gemItem : null;

            MaxHP = BaseEntityBattleStats.BASE_MAX_HP;
            foreach (var accessory in accessories)
                if (accessory)
                    MaxHP += accessory.ExtraMaxHP;
            if (gem)
                MaxHP += gem.ExtraMaxHP;

            MaxCurse = BaseEntityBattleStats.BASE_MAX_CURSE;
            foreach (var accessory in accessories)
                if (accessory)
                    MaxCurse += accessory.ExtraMaxCurse;
            if (gem)
                MaxCurse += gem.ExtraMaxCurse;


            CurrentHP = currentHP;
            CurrentCurse = currentCurse;
        }
        public void SetFromSave(int maxHP, int maxCurse, int currentHP, int currentCurse)
        {
            MaxHP = maxHP;
            MaxCurse = maxCurse;
            CurrentHP = currentHP;
            CurrentCurse = currentCurse;
        }

        public PlayerBattleData() { }
        public PlayerBattleData(PlayerBattleData other)
        {
            MaxHP = other.MaxHP;
            MaxCurse = other.MaxCurse;
            CurrentHP = other.CurrentHP;
            CurrentCurse = other.CurrentCurse;
        }

    }
}