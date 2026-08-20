using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Puzzle
{
    public class SlidingPipeView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public PipePieceInstance PieceData { get; private set; }
        private PipePuzzleView _owner;
        private Vector2 _dragStart;
        private const float SwipeThreshold = 30f;

        [SerializeField] private RectTransform visualRoot;
        [SerializeField] private Image pipeImage;
        [SerializeField] private PipeSpriteConfig spriteConfig;

        
        public void Init(PipePieceInstance data, PipePuzzleView owner)
        {
            PieceData = data;
            _owner = owner;
            RenderVisual();
        }

        public void RenderVisual()
        {

            var sprite = spriteConfig.GetSprite(PieceData.pipeName);
            pipeImage.sprite = sprite;
            visualRoot.localRotation = Quaternion.Euler(0, 0, -90f * PieceData.rotation);
        }

        public void OnBeginDrag(PointerEventData eventData) => _dragStart = eventData.position;

        public void OnDrag(PointerEventData eventData) { }
        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.position - _dragStart;
            if (delta.magnitude < SwipeThreshold) return;

            SlideDirection direction = Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
                ? (delta.x > 0 ? SlideDirection.Right : SlideDirection.Left)
                : (delta.y > 0 ? SlideDirection.Up : SlideDirection.Down);

            _owner.SlidePiece(this, direction);
        }

        public void OnPointerClick(PointerEventData eventData) => _owner.RotatePiece(this);
    }
}