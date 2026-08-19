using UnityEngine;

namespace Game.SO.Data.Item.Sellable
{
    [CreateAssetMenu(fileName = "ConsumableItem_Data", menuName = "Scriptable Objects/Data/Item/Sellable/ConsumableItemSO")]
    public class ConsumableItemSO : SellableItemSO
    {

        public enum EFFECT
        {
            HEAL,
            DAMAGE_BOOST,
            CURSE_REGENERATE
        }

        [field: SerializeField]
        public EFFECT Effect { get; private set; }
        [field: SerializeField]
        public float Value { get; private set; }


        public void ConsumeBy(int placeHolderEntity)
        {
            switch (Effect)
            {
                case EFFECT.HEAL:
                    break;
                case EFFECT.DAMAGE_BOOST:
                    break;
                case EFFECT.CURSE_REGENERATE:
                    break;
                default:
                    break;
            }
        }

    }
}