using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Puzzle;

namespace Puzzle
{
    public class GridRenderer : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private RectTransform gridContainerRect;
        [SerializeField] private RectTransform gridContainer;
        [SerializeField] private GameObject cellBackgroundPrefab;
        public RectTransform shapeContainer;
        [SerializeField] private RectTransform rowClueContainer;
        [SerializeField] private RectTransform colClueContainer;
        [SerializeField] private GameObject clueLabelPrefab;
        private float cellSize;

        [SerializeField] private GameObject ghostCellPrefab;
        [SerializeField] private Color validColor = new Color(0, 1, 0, 0.4f);
        [SerializeField] private Color invalidColor = new Color(1, 0, 0, 0.4f);

        private List<GameObject> _ghostCells = new();

        public RectTransform ShapeContainer => shapeContainer;
        public float CellSize => cellSize;

        public void RenderGrid(int rows, int cols, Clue[] rowRuns, Clue[] colRuns)
        {
            ApplyDynamicCellSize(rows, cols);

            foreach (Transform child in gridContainer) Destroy(child.gameObject);
            for (int i = 0; i < rows * cols; i++)
                Instantiate(cellBackgroundPrefab, gridContainer);

            RenderClues(rowClueContainer, rowRuns);
            RenderClues(colClueContainer, colRuns);
        }

        private void ApplyDynamicCellSize(int rows, int cols)
        {
            float availableWidth = gridContainerRect.rect.width;
            float availableHeight = gridContainerRect.rect.height;

            float cellWidthFit = availableWidth / cols;
            float cellHeightFit = availableHeight / rows;

            cellSize = Mathf.Min(cellWidthFit, cellHeightFit);
            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = cols;

            rowClueContainer.sizeDelta = new Vector2(rowClueContainer.sizeDelta.x, rows * cellSize);
            colClueContainer.sizeDelta = new Vector2(cols * cellSize, colClueContainer.sizeDelta.y);
        }

        private void RenderClues(RectTransform container, Clue[] clues)
        {
            if (container == null) { Debug.LogError($"[{name}] RenderClues: container is null", this); return; }
            if (clueLabelPrefab == null) { Debug.LogError($"[{name}] RenderClues: clueLabelPrefab not assigned", this); return; }
            if (clues == null) { Debug.LogError($"[{name}] RenderClues: clues array is null", this); return; }

            

            foreach (Transform child in container) 
                Destroy(child.gameObject);

            if (clues == null)
                return;

            foreach (var clue in clues)
            {
                var label = Instantiate(clueLabelPrefab, container).GetComponentInChildren<TMP_Text>();
                label.text = string.Join(" ", clue.run);
            }
        }

        public void ShowGhost(Vector2Int anchor, Vector2Int[] shapeOffsets, bool isValid)
        {
            ClearGhost();
            foreach (var offset in shapeOffsets)
            {
                var cell = anchor + offset;
                var ghostGO = Instantiate(ghostCellPrefab, shapeContainer);
                var rect = ghostGO.GetComponent<RectTransform>();
                rect.anchoredPosition = GridToLocalPosition(cell);
                rect.sizeDelta = new Vector2(cellSize, cellSize);
                rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
                ghostGO.GetComponent<Image>().color = isValid ? validColor : invalidColor;
                _ghostCells.Add(ghostGO);
            }
        }

        public void ClearGhost()
        {
            foreach (var go in _ghostCells)
            {
                Destroy(go);
            }

            _ghostCells.Clear();
        }

        public Vector2 GridToLocalPosition(Vector2Int gridCoord) => new Vector2(gridCoord.x * cellSize, -gridCoord.y * cellSize);

        public Vector2Int LocalPositionToGrid(Vector2 localPos) => new Vector2Int(Mathf.FloorToInt(localPos.x / cellSize), Mathf.FloorToInt(-localPos.y / cellSize));
    }       
}