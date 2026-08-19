using System;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// "spam Z and X for 2 seconds" flee attempt. Required press count is (sum of all enemies' attack + defense) * 2, a failed attempt keeps its progress so the next attempt within the same battle needs fewer presses.
    /// </summary>
    public class FleeMinigameController : MonoBehaviour
    {
        [SerializeField] float attemptDuration = 2f;

        CombatInputReader input;
        bool running;
        int requiredPresses;
        int pressesSoFar; // carries over across failed attempts within one battle
        float timer;
        Action<bool> onResult; // true = escaped

        public void Init(CombatInputReader inputReader)
        {
            input = inputReader;
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
        }

        void HandlePress(bool isZ)
        {
            if (!running) return;
            pressesSoFar++;
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

            if (success)
                pressesSoFar = 0; // battle's over anyway, but reset for cleanliness
            // on failure pressesSoFar is deliberately NOT reset, so the next attempt is easier

            onResult?.Invoke(success);
        }

        /// <summary>0-1, useful for driving a progress bar UI</summary>
        public float Progress01 => requiredPresses <= 0 ? 0 : Mathf.Clamp01((float)pressesSoFar / requiredPresses);
        public float TimeRemaining01 => attemptDuration <= 0 ? 0 : Mathf.Clamp01(timer / attemptDuration);
    }
}
