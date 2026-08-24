using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle
{
    public class DraggableBall : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public string BallId;

        private RectTransform _rect;
        private RectTransform _trayContainer;
        private Vector2 _trayPosition;
        private CanvasGroup _canvasGroup;
        private BallSlotView _currentSlot;
        private BalancePuzzleView _owner;
        private bool _placedThisDrag;

        public void Init(string ballId, RectTransform trayContainer, Canvas canvas, BalancePuzzleView owner)
        {
            BallId = ballId;
            _trayContainer = trayContainer;
            _owner = owner;
            _rect = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _trayPosition = _rect.anchoredPosition;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _placedThisDrag = false;
            if (_canvasGroup != null) _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData) => _rect.position = eventData.position;

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_canvasGroup != null) _canvasGroup.blocksRaycasts = true;

            if (!_placedThisDrag)
            {
                if (_currentSlot != null)
                {
                    _currentSlot.SetOccupant(null);
                    _owner.OnBallRemoved(_currentSlot.SlotId);
                    _currentSlot = null;
                }

                _rect.SetParent(_trayContainer, worldPositionStays: false);
                _rect.anchoredPosition = _trayPosition;
            }
        }

        public void PlaceInSlot(BallSlotView slot)
        {
            _placedThisDrag = true;

            if (_currentSlot != null && _currentSlot != slot)
            {
                _currentSlot.SetOccupant(null);
                _owner.OnBallRemoved(_currentSlot.SlotId);
            }

            _currentSlot = slot;
            slot.SetOccupant(this);

            _rect.SetParent(slot.transform, worldPositionStays: false);
            _rect.anchorMin = new Vector2(0.5f, 0.5f);
            _rect.anchorMax = new Vector2(0.5f, 0.5f);
            _rect.pivot = new Vector2(0.5f, 0.5f);
            _rect.anchoredPosition = Vector2.zero;

            _owner.OnBallPlaced(slot.SlotId, BallId);
        }
    }
}