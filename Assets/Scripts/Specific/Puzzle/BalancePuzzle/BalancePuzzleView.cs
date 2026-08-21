using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle
{
    public class BalancePuzzleView : MonoBehaviour, IPuzzleView
    {
        public event Action<bool> OnSolutionSubmitted;

        [SerializeField] private RectTransform structureVisual; 
        [SerializeField] private RectTransform slotContainer;   
        [SerializeField] private GameObject ballSlotPrefab;
        [SerializeField] private RectTransform trayContainer;
        [SerializeField] private GameObject draggableBallPrefab;
        [SerializeField] private TMP_Text offsetReadout;
        [SerializeField] private float pixelsPerUnit = 50f; 

        private BalancePuzzleData _data;
        private List<BallSlotView> _slotViews = new();
        private Dictionary<string, string> _placement = new();

        public void Load(PuzzleData data)
        {
            _data = data as BalancePuzzleData;
            _placement.Clear();

            foreach (Transform child in slotContainer) Destroy(child.gameObject);
            _slotViews.Clear();
            foreach (var slot in _data.slots)
            {
                var slotGO = Instantiate(ballSlotPrefab, slotContainer);
                var slotView = slotGO.GetComponent<BallSlotView>();
                slotView.SlotId = slot.slotId;
                slotGO.GetComponent<RectTransform>().anchoredPosition = slot.localPosition * pixelsPerUnit;
                _slotViews.Add(slotView);
            }

            foreach (Transform child in trayContainer) Destroy(child.gameObject);
            foreach (var ball in _data.balls)
            {
                var ballGO = Instantiate(draggableBallPrefab, trayContainer);
                ballGO.GetComponent<DraggableBall>().Init(ball.ballId, trayContainer, GetComponentInParent<Canvas>(), this);
                ballGO.GetComponentInChildren<TMP_Text>().text = $"{ball.ballId} ({ball.mass}kg)";
            }

            UpdateFeedback();
        }

        public void OnBallPlaced(string slotId, string ballId)
        {
            _placement[slotId] = ballId;
            UpdateFeedback();
        }

        public void OnBallRemoved(string slotId)
        {
            _placement.Remove(slotId);
            UpdateFeedback();
        }

        private void UpdateFeedback()
        {
            Vector2 currentCoM = _data.ComputeCenterOfMass(_placement);

            float tiltX = Mathf.Clamp(currentCoM.x * 10f, -25f, 25f);
            float tiltY = Mathf.Clamp(currentCoM.y * 10f, -25f, 25f);
            structureVisual.localRotation = Quaternion.Euler(tiltY, 0, -tiltX);

            if (offsetReadout != null)
                offsetReadout.text = $"Offset: ({currentCoM.x:F2}, {currentCoM.y:F2}) / Target: ({_data.targetOffset.x:F2}, {_data.targetOffset.y:F2})";

            bool balanced = _data.CheckSolution(_placement);
            if (balanced)
                OnSolutionSubmitted?.Invoke(true);
        }
    }
}
