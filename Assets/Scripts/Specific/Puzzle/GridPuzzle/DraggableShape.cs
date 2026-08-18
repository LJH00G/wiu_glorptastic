using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Puzzle
{
    public class DraggableShape : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public PolyominoShape shapeData;
        public int rotation;

        public bool IsPlaced => _placedAnchor.HasValue;
        public Vector2Int PlacedAnchor => _placedAnchor.Value;

        [SerializeField] private GameObject cellSpritePrefab;
        [SerializeField] private float cellSize = 64f;

        private RectTransform _rect;
        private GridRenderer _gridRenderer;
        private ShapeFitPuzzleView _owner;
        private Vector2 _trayPosition;
        private Vector2Int? _placedAnchor;
        private RectTransform _trayContainer;

        private List<GameObject> _cellVisuals = new();
        public void Init(PolyominoShape data, GridRenderer gridRenderer, ShapeFitPuzzleView owner, RectTransform trayContainer)
        {
            shapeData = data;
            _gridRenderer = gridRenderer;
            _owner = owner;
            _rect = GetComponent<RectTransform>();
            _trayContainer = trayContainer;
            _trayPosition = _rect.anchoredPosition;
            RenderShapeCells();
        }

        private void RenderShapeCells()
        {
            foreach (var go in _cellVisuals) Destroy(go);
            _cellVisuals.Clear();

            foreach (var offset in shapeData.Rotated(rotation))
            {
                var cellGO = Instantiate(cellSpritePrefab, transform);
                var rect = cellGO.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(offset.x * cellSize, -offset.y * cellSize);
                _cellVisuals.Add(cellGO);
            }
        }

        public void OnBeginDrag(PointerEventData eventData) { }

        public void OnDrag(PointerEventData eventData)
        {
            _rect.position = eventData.position;
            _owner.ShowGhostPreview(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var gridCoord = _owner.ScreenPointToGridCoord(eventData);

            if (_owner.TryPlace(this, gridCoord, rotation))
            {
                _placedAnchor = gridCoord;
                _rect.SetParent(_gridRenderer.shapeContainer, worldPositionStays: false);
                _rect.anchoredPosition = _gridRenderer.GridToLocalPosition(gridCoord);
            }
            else
            {
                _placedAnchor = null;
                _rect.SetParent(_trayContainer, worldPositionStays: false);
                _rect.anchoredPosition = _trayPosition;
            }
            _owner.HideGhostPreview();
        }

        public void Rotate()
        {
            rotation = (rotation + 1) % 4;
            RenderShapeCells();
            if (_placedAnchor.HasValue && !_owner.TryPlace(this, _placedAnchor.Value, rotation))
            {
                
                _placedAnchor = null;
                _rect.anchoredPosition = _trayPosition;
            }
        }
    }
}