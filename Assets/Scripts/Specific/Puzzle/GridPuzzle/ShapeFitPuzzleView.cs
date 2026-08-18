using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle
{
    public class ShapeFitPuzzleView : MonoBehaviour, IPuzzleView
    {
        public event Action<bool> OnSolutionSubmitted;

        [SerializeField] private GridRenderer gridRenderer;
        [SerializeField] private RectTransform trayContainer;
        [SerializeField] private GameObject draggableShapePrefab;
        [SerializeField] private Button submitButton;

        private ShapeFitPuzzleData _data;
        private List<DraggableShape> _shapes = new();

        public void Load(PuzzleData data)
        {
            _data = data as ShapeFitPuzzleData;
            gridRenderer.RenderGrid(_data.rows, _data.cols, _data.rowRuns, _data.colRuns);

            foreach (Transform child in trayContainer)
            {
                Destroy(child.gameObject);
            }
            
            _shapes.Clear();

            foreach (var shapeDef in _data.availableShapes)
            {
                var obj = Instantiate(draggableShapePrefab, trayContainer);
                var draggable = obj.GetComponent<DraggableShape>();
                draggable.Init(shapeDef, gridRenderer, this);
                _shapes.Add(draggable);
            }

            submitButton.onClick.AddListener(OnSubmit);
        }

        public bool TryPlace(DraggableShape shape, Vector2Int anchor, int rotation)
        {
            var existingFilled = BuildFilledGridExcluding(shape);
            var placement = new ShapePlacement {shapeName = shape.shapeData.shapeName, anchor = anchor, rotation = rotation};

            return PlacementValidator.IsValidPlacement(placement, shape.shapeData, _data.rows, _data.cols, existingFilled);
        }

        private bool[,] BuildFilledGridExcluding(DraggableShape excluded)
        {
            var placements = _shapes.Where(s => s.IsPlaced && s != excluded).Select(s => new ShapePlacement { shapeName = s.shapeData.shapeName, anchor = s.PlacedAnchor, rotation = s.rotation }).ToList();

            return PlacementValidator.BuildFilledGrid(placements, _data.availableShapes, _data.rows, _data.cols) ?? new bool[_data.rows, _data.cols]; 
        }

        public void ShowGhostPreview(DraggableShape shape, Vector2 screenPos)
        {
            var localPos = gridRenderer.shapeContainer.InverseTransformPoint(screenPos);
            var anchor = gridRenderer.LocalPositionToGrid(localPos);
            var offsets = shape.shapeData.Rotated(shape.rotation);

            bool isValid = TryPlace(shape, anchor, shape.rotation); 

            gridRenderer.ShowGhost(anchor, offsets, isValid);
        }

        public void HideGhostPreview()
        {
            gridRenderer.ClearGhost();
        } 

        private void OnSubmit()
        {
            var placements = _shapes.Where(s => s.IsPlaced).Select(s => new ShapePlacement { shapeName = s.shapeData.shapeName, anchor = s.PlacedAnchor, rotation = s.rotation }).ToList();

            bool correct = _data.CheckSolution(placements);
            OnSolutionSubmitted?.Invoke(correct);
        }
    }
}