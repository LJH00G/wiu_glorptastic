using UnityEngine;
using UnityEngine.UI;

namespace Puzzle
{
    public class PipeGridRenderer : MonoBehaviour
    {
        [SerializeField] private RectTransform gridContainer;   
        [SerializeField] private GameObject cellBackgroundPrefab;
        [SerializeField] private RectTransform pieceContainer;  

        private float cellSize;
        public RectTransform PieceContainer => pieceContainer;
        public float CellSize => cellSize;

        public void RenderGrid(int rows, int cols)
        {
            ApplyDynamicCellSize(rows, cols);

            foreach (Transform child in gridContainer) Destroy(child.gameObject);
            for (int i = 0; i < rows * cols; i++)
                Instantiate(cellBackgroundPrefab, gridContainer);
        }

        private void ApplyDynamicCellSize(int rows, int cols)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridContainer);

            float availableWidth = gridContainer.rect.width;
            float availableHeight = gridContainer.rect.height;

            float cellWidthFit = availableWidth / cols;
            float cellHeightFit = availableHeight / rows;
            cellSize = Mathf.Min(cellWidthFit, cellHeightFit);

            var layoutGroup = gridContainer.GetComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(cellSize, cellSize);
        }

        public Vector2 GridToLocalPosition(Vector2Int gridCoord) =>
            new Vector2(gridCoord.x * cellSize, -gridCoord.y * cellSize);

        public Vector2Int LocalPositionToGrid(Vector2 localPos) =>
            new Vector2Int(Mathf.RoundToInt(localPos.x / cellSize), Mathf.RoundToInt(-localPos.y / cellSize));
    }
}