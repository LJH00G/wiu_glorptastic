using UnityEngine;
using Utility.DictionaryEntry;
using System.Collections.Generic;
using System;
using Game.SO.EventChannel;


namespace Puzzle
{
    [Serializable]
    public enum PuzzleType
    {
        None,
        Base
    }

    public class PuzzleHandler : MonoBehaviour
    {

        [SerializeField] Transform container;
        [SerializeField] List<DictEntry<PuzzleType, GameObject>> viewRegistry = new();
        [SerializeField] EventChannelSO OnCompletePuzzle;

        private IPuzzleView _activeView;
        private GameObject _activeViewInstance;
        private Dictionary<PuzzleType, GameObject> _viewDict = new();

        private void Awake()
        {
            BuildViewDict();
        }

        private void BuildViewDict()
        {
            _viewDict = new Dictionary<PuzzleType, GameObject>();
            foreach (var entry in viewRegistry)
            {
                if (entry.value == null)
                {
                    Debug.LogWarning($"[{name}] Missing prefab for {entry.key}", this);
                    continue;
                }

                if (!_viewDict.TryAdd(entry.key, entry.value))
                {
                    Debug.LogError($"[{name}] Duplicate registry entry for {entry.key}", this);
                }
                    
            }
        }

        public void LoadPuzzle(PuzzleData data)
        {
            
            if (_viewDict == null) BuildViewDict(); 

            if (!_viewDict.TryGetValue(data.puzzleType, out var prefab))
            {
                Debug.LogError($"No view prefab registered for puzzle type: {data.puzzleType}", this);
                return;
            }

            UnloadCurrentView();


            _activeViewInstance = Instantiate(prefab, container);
            _activeView = _activeViewInstance.GetComponent<IPuzzleView>();

            if (_activeView == null)
            {
                Debug.LogError($"Prefab '{prefab.name}' has no IPuzzleView component", this);
                Destroy(_activeViewInstance);
                return;
            }

            _activeView.OnSolutionSubmitted += HandleResult;
            _activeView.Load(data);
        }

        private void UnloadCurrentView()
        {
            if (_activeView != null)
                _activeView.OnSolutionSubmitted -= HandleResult;

            if (_activeViewInstance != null)
                Destroy(_activeViewInstance);

            _activeView = null;
            _activeViewInstance = null;
        }

        private void HandleResult(bool correct)
        {
            if(correct)
            {
                UnloadCurrentView();
                OnCompletePuzzle.Raise();
                return;
            }
        }

        private void OnDestroy()
        {
            UnloadCurrentView();
        }
    }

}
