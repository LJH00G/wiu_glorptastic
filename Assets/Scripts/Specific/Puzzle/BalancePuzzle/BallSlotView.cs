using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle
{
    public class BallSlotView : MonoBehaviour, IDropHandler
    {
        public string SlotId;
        public string OccupantId { get; private set; }
        private DraggableBall _occupant;
        private BalancePuzzleView _owner;

        public void Init(BalancePuzzleView owner) => _owner = owner;

        public void OnDrop(PointerEventData eventData)
        {
            var dragged = eventData.pointerDrag?.GetComponent<DraggableBall>();
            if (dragged == null) 
                return;

            if (_occupant != null && _occupant != dragged)
            {
                return; 
            }

            dragged.PlaceInSlot(this);
        }

        public void SetOccupant(DraggableBall ball)
        {
            _occupant = ball;
            OccupantId = ball?.BallId;
        }
    }
}