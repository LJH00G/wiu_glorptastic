using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Puzzle;

namespace Puzzle
{
    public class GridRenderer : MonoBehaviour
    {
        [SerializeField] private RectTransform gridContainer;
        [SerializeField] private GameObject cellBackgroundPrefab;
        public RectTransform shapeContainer;
        [SerializeField] private RectTransform rowClueContainer;
        [SerializeField] private RectTransform colClueContainer;
        [SerializeField] private GameObject clueLabelPrefab;
        public float cellSize = 64f;

        [SerializeField] private GameObject ghostCellPrefab;
        [SerializeField] private Color validColor = new Color(0, 1, 0, 0.4f);
        [SerializeField] private Color invalidColor = new Color(1, 0, 0, 0.4f);

        private List<GameObject> _ghostCells = new();


        public void RenderGrid(int rows, int cols, Clue[] rowRuns, Clue[] colRuns)
        {
            foreach (Transform child in gridContainer) Destroy(child.gameObject);
            for (int i = 0; i < rows * cols; i++)
                Instantiate(cellBackgroundPrefab, gridContainer);

            RenderClues(rowClueContainer, rowRuns);
            RenderClues(colClueContainer, colRuns);
        }

        private void RenderClues(RectTransform container, Clue[] clues)
        {
            

            foreach (Transform child in container) Destroy(child.gameObject);
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

        public Vector2 GridToLocalPosition(Vector2Int gridCoord)
        {
            return new Vector2(gridCoord.x * cellSize, -gridCoord.y * cellSize);
        }

        public Vector2Int LocalPositionToGrid(Vector2 localPos)
        {
            return new Vector2Int(Mathf.RoundToInt(localPos.x / cellSize), Mathf.RoundToInt(-localPos.y / cellSize));
        }
    }       
}