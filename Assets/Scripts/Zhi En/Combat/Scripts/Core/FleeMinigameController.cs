using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Combat
{
    /// <summary>
    /// "spam Z and X for 2 seconds" flee attempt. Required press count is
    /// (sum of all enemies' attack + defense) * 2 a failed attempt keeps
    /// its progress so the next attempt within the same battle needs fewer presses
    /// </summary>
    public class FleeMinigameController : MonoBehaviour
    {
        [SerializeField] float attemptDuration = 2f;

        [Header("UI (positioned roughly where the attack mastery bar sits)")]
        [SerializeField] GameObject barRoot;
        [SerializeField] Image fillImage;

        CombatInputReader input;
        bool running;
        int requiredPresses;
        int pressesSoFar; // carries over across failed attempts within one battle
        float timer;
        Action<bool> onResult; // true = escaped

        public void Init(CombatInputReader inputReader)
        {
            input = inputReader;
            if (barRoot) barRoot.SetActive(false);
        }

        /// <summary>call once per battle when a new set of enemies is loaded, or whenever their combined stats change</summary>
        public void SetRequiredPressesFromEnemies(int combinedAtkPlusDef)
        {
            requiredPresses = Mathf.Max(combinedAtkPlusDef * 2, 1);
        }

        public void BeginAttempt(Action<bool> resultCallback)
        {
            onResult = resultCallback;
            timer = attemptDuration;
            running = true;
            input.OnConfirmOrCancelHeldThisFrame.Subscribe(HandlePress);

            if (barRoot) barRoot.SetActive(true);
            RefreshBar(); // shows the carried-over progress immediately, even before the first new press
        }

        void HandlePress(bool isZ)
        {
            if (!running) return;
            pressesSoFar++;
            RefreshBar();
        }

        void Update()
        {
            if (!running) return;

            timer -= Time.unscaledDeltaTime;

            int remaining = Mathf.Max(requiredPresses - pressesSoFar, 0);

            if (remaining <= 0)
            {
                FinishAttempt(true);
                return;
            }

            if (timer <= 0f)
            {
                FinishAttempt(false);
            }
        }

        void FinishAttempt(bool success)
        {
            running = false;
            input.OnConfirmOrCancelHeldThisFrame.Unsubscribe(HandlePress);

            if (barRoot) barRoot.SetActive(false);

            if (success)
                pressesSoFar = 0;

            onResult?.Invoke(success);
        }

        void RefreshBar()
        {
            if (fillImage) fillImage.fillAmount = Progress01;
        }

        /// <summary>0-1, useful for driving a progress bar UI</summary>
        public float Progress01 => requiredPresses <= 0 ? 0 : Mathf.Clamp01((float)pressesSoFar / requiredPresses);
        public float TimeRemaining01 => attemptDuration <= 0 ? 0 : Mathf.Clamp01(timer / attemptDuration);
    }
}
