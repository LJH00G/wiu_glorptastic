using UnityEngine;

using UnityEngine.EventSystems;

namespace Puzzle
{
    public class DraggablePerson : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public string PersonId;

        private RectTransform _rect;
        private RectTransform _trayContainer;
        private Vector2 _trayPosition;
        private SeatSlot _currentSeat;
        private Canvas _canvas;

        [SerializeField] private CanvasGroup canvasGroup;
        public void Init(string personId, RectTransform trayContainer, Canvas canvas)
        {
            PersonId = personId;
            _trayContainer = trayContainer;
            _canvas = canvas;
            _rect = GetComponent<RectTransform>();
            _trayPosition = _rect.anchoredPosition;
        }

        

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = false; 
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;

            if (_currentSeat == null)
            {
                _rect.SetParent(_trayContainer, worldPositionStays: false);
                _rect.anchoredPosition = _trayPosition; 
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rect.position = eventData.position;
        }

        

        public void PlaceInSeat(SeatSlot seat)
        {
            _currentSeat?.SetOccupant(null); 
            _currentSeat = seat;
            seat.SetOccupant(this);

            _rect.SetParent(seat.transform, worldPositionStays: false);
            _rect.anchoredPosition = Vector2.zero;

            _rect.anchorMin = new Vector2(0.5f, 0.5f);
            _rect.anchorMax = new Vector2(0.5f, 0.5f);
            _rect.pivot = new Vector2(0.5f, 0.5f);
            
        }
    }
}
