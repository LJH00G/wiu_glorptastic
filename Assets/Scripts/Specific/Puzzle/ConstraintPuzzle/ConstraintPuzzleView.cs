using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Puzzle
{
    public class ConstraintPuzzleView : MonoBehaviour, IPuzzleView
    {
        public event Action<bool> OnSolutionSubmitted;

        [SerializeField] private RectTransform seatGridContainer; 
        [SerializeField] private GameObject seatSlotPrefab;
        [SerializeField] private RectTransform trayContainer;
        [SerializeField] private GameObject draggablePersonPrefab;
        [SerializeField] private TMP_Text clueText;
        [SerializeField] private Button submitButton;

        [SerializeField] private GridLayoutGroup seatGridLayoutGroup;

        private ConstraintPuzzleData _data;
        private List<SeatSlot> _seatSlots = new();
        private Canvas _canvas;

        public void Load(PuzzleData data)
        {
            _data = data as ConstraintPuzzleData;
            _canvas = GetComponentInParent<Canvas>();
            ApplyDynamicSeatCellSize(_data.layout.rows, _data.layout.cols);
            foreach (Transform child in seatGridContainer)
            {
                Destroy(child.gameObject);
            }
                
            _seatSlots.Clear();

            foreach (var seatId in _data.layout.seatID)
            {
                if (string.IsNullOrEmpty(seatId)) 
                { 
                    Instantiate(seatSlotPrefab, seatGridContainer).SetActive(false); 
                    continue; 
                } 

                var slotGO = Instantiate(seatSlotPrefab, seatGridContainer);
                var slot = slotGO.GetComponent<SeatSlot>();
                slot.SeatId = seatId;
                _seatSlots.Add(slot);
            }

            foreach (Transform child in trayContainer) 
                Destroy(child.gameObject);

            foreach (var personId in _data.people)
            {
                var personGO = Instantiate(draggablePersonPrefab, trayContainer);
                personGO.GetComponent<DraggablePerson>().Init(personId, trayContainer, _canvas);
                personGO.GetComponentInChildren<TMP_Text>().text = personId;
            }

            clueText.text = string.Join("\n", _data.constraints.Select(c => "• " + c.Describe()));
            submitButton.onClick.AddListener(OnSubmit);
        }

        private void ApplyDynamicSeatCellSize(int rows, int cols)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(seatGridContainer);

            float availableWidth = seatGridContainer.rect.width;
            float availableHeight = seatGridContainer.rect.height;

            float cellWidthFit = availableWidth / cols;
            float cellHeightFit = availableHeight / rows;
            float finalSize = Mathf.Min(cellWidthFit, cellHeightFit);

            seatGridLayoutGroup.cellSize = new Vector2(finalSize, finalSize);
        }

        private void OnSubmit()
        {
            var arrangement = new Dictionary<string, string>();
            foreach (var slot in _seatSlots)
            {
                if (!string.IsNullOrEmpty(slot.OccupantId))
                    arrangement[slot.SeatId] = slot.OccupantId;
            }

            bool correct = _data.CheckSolution(arrangement);
            OnSolutionSubmitted?.Invoke(correct);
        }
    }
}
