using Puzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle
{
    public class PipePuzzleView : MonoBehaviour, IPuzzleView
    {
        public event Action<bool> OnSolutionSubmitted;

        [SerializeField] private PipeGridRenderer gridRenderer;
        [SerializeField] private GameObject slidingPipePrefab;
        [SerializeField] private GameObject sourceMarkerPrefab;
        [SerializeField] private GameObject targetMarkerPrefab;
        [SerializeField] private TextMeshProUGUI statusText;

        private WaterPuzzleData _data;
        private List<SlidingPipeView> _pieceViews = new();
        private GameObject _sourceMarkerInstance;
        private GameObject _targetMarkerInstance;

        public void Load(PuzzleData data)
        {
            _data = data as WaterPuzzleData;

            gridRenderer.RenderGrid(_data.rows, _data.cols);

            SpawnMarkers();
            SpawnPieces();

            CheckLive();
        }

        private void SpawnMarkers()
        {
            if (_sourceMarkerInstance != null) Destroy(_sourceMarkerInstance);
            if (_targetMarkerInstance != null) Destroy(_targetMarkerInstance);

            if (sourceMarkerPrefab == null || targetMarkerPrefab == null)
            {
                Debug.LogWarning($"[{name}] Source/target marker prefabs not assigned — puzzle will render without them", this);
                return;
            }

            var cellSize = new Vector2(gridRenderer.CellSize, gridRenderer.CellSize);

            _sourceMarkerInstance = Instantiate(sourceMarkerPrefab, gridRenderer.PieceContainer);

            var markerRect = _sourceMarkerInstance.GetComponent<RectTransform>();
            markerRect.anchorMin = new Vector2(0f, 1f);
            markerRect.anchorMax = new Vector2(0f, 1f);
            markerRect.pivot = new Vector2(0f, 1f);
            markerRect.sizeDelta = new Vector2(gridRenderer.CellSize, gridRenderer.CellSize);
            markerRect.anchoredPosition = gridRenderer.GridToLocalPosition(_data.sourceCell);
            _sourceMarkerInstance.transform.SetAsFirstSibling();

            _targetMarkerInstance = Instantiate(targetMarkerPrefab, gridRenderer.PieceContainer);
                
            markerRect = _targetMarkerInstance.GetComponent<RectTransform>();
            markerRect.anchorMin = new Vector2(0f, 1f);
            markerRect.anchorMax = new Vector2(0f, 1f);
            markerRect.pivot = new Vector2(0f, 1f);
            markerRect.sizeDelta = new Vector2(gridRenderer.CellSize, gridRenderer.CellSize);
            markerRect.anchoredPosition = gridRenderer.GridToLocalPosition(_data.targetCell);
            _targetMarkerInstance.transform.SetAsFirstSibling();
        }

        private void SpawnPieces()
        {
            foreach (var view in _pieceViews) Destroy(view.gameObject);
            _pieceViews.Clear();

            foreach (var startPiece in _data.startingPieces)
            {
                var pieceCopy = new PipePieceInstance
                {
                    pieceId = startPiece.pieceId,
                    pipeName = startPiece.pipeName,
                    position = startPiece.position,
                    rotation = startPiece.rotation
                };

                var go = Instantiate(slidingPipePrefab, gridRenderer.PieceContainer);

                var rect = go.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.sizeDelta = new Vector2(gridRenderer.CellSize, gridRenderer.CellSize);
                rect.anchoredPosition = gridRenderer.GridToLocalPosition(pieceCopy.position);

                var pieceView = go.GetComponent<SlidingPipeView>();
                pieceView.Init(pieceCopy, this);

                _pieceViews.Add(pieceView);
            }
        }

        public void SlidePiece(SlidingPipeView pieceView, SlideDirection direction)
        {
            var allData = _pieceViews.Select(v => v.PieceData).ToList();
            var newPos = SlideValidator.GetOneStepMove(pieceView.PieceData, direction, allData, _data.rows, _data.cols);

            pieceView.PieceData.position = newPos;
            var rect = pieceView.GetComponent<RectTransform>();
            rect.anchoredPosition = gridRenderer.GridToLocalPosition(newPos);

            CheckLive();
        }

        public void RotatePiece(SlidingPipeView pieceView)
        {
            pieceView.PieceData.rotation = (pieceView.PieceData.rotation + 1) % 4;
            pieceView.RenderVisual();
            CheckLive();
        }

        private void CheckLive()
        {
            var allData = _pieceViews.Select(v => v.PieceData).ToList();
            bool solved = _data.CheckSolution(allData);

            if (statusText != null)
                statusText.text = solved ? "Connected!" : "";
                
            if (solved)
                OnSolutionSubmitted?.Invoke(true);
        }
    }
}