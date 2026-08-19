using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle
{
    [System.Serializable]
    public class SeatSlot : MonoBehaviour, IDropHandler
    {
        public string SeatId;
        public string OccupantId { get; private set; }
        private DraggablePerson _occupant;

        public void OnDrop(PointerEventData eventData)
        {
            var dragged = eventData.pointerDrag?.GetComponent<DraggablePerson>();
            if (dragged == null) 
                return;

            dragged.PlaceInSeat(this);
        }

        public void SetOccupant(DraggablePerson person)
        {
            _occupant = person;
            OccupantId = person?.PersonId;
        }


    }
}
