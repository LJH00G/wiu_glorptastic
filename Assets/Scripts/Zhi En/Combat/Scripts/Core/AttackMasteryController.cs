using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Combat
{
    /// <summary>
    /// low-priority polish feature: a bar with a line sweeping left-to-right; pressing Z while
    /// the line is inside the green window gives +1 damage (on the player's attack) or
    /// -1 damage taken (on an incoming enemy attack). Kept intentionally simple - the doc calls
    /// this out as "make last".
    /// </summary>
    public class AttackMasteryController : MonoBehaviour
    {
        [SerializeField] RectTransform bar;
        [SerializeField] RectTransform line;
        [SerializeField] Image windowImage; // the green zone, resized/positioned per attempt
        [SerializeField] float sweepDuration = 0.8f;

        CombatInputReader input;
        bool running;
        float timer;
        float windowStart01, windowEnd01; // normalised 0-1 position along the bar
        bool hitRegistered;
        Action<bool> onResult;

        public void Init(CombatInputReader inputReader)
        {
            input = inputReader;
        }

        public void BeginAttempt(float windowWidth01, Action<bool> resultCallback)
        {
            onResult = resultCallback;
            windowWidth01 = Mathf.Clamp01(windowWidth01); // fraction of bar the green window covers

            windowStart01 = UnityEngine.Random.Range(0f, 1f - windowWidth01);
            windowEnd01 = windowStart01 + windowWidth01;

            if (windowImage)
            {
                var rt = windowImage.rectTransform;
                rt.anchorMin = new Vector2(windowStart01, rt.anchorMin.y);
                rt.anchorMax = new Vector2(windowEnd01, rt.anchorMax.y);
            }

            timer = 0f;
            hitRegistered = false;
            running = true;
            gameObject.SetActive(true);
            input.OnConfirmPressed.Subscribe(HandleConfirm);
        }

        void HandleConfirm()
        {
            if (!running || hitRegistered) return;
            hitRegistered = true;

            float t01 = timer / sweepDuration;
            bool success = t01 >= windowStart01 && t01 <= windowEnd01;
            Finish(success);
        }

        void Update()
        {
            if (!running) return;

            timer += Time.unscaledDeltaTime;
            float t01 = Mathf.Clamp01(timer / sweepDuration);

            if (bar && line)
                line.anchoredPosition = new Vector2(Mathf.Lerp(0, bar.rect.width, t01), line.anchoredPosition.y);

            if (timer >= sweepDuration && !hitRegistered)
            {
                hitRegistered = true;
                Finish(false); // ran out of time without pressing = no bonus
            }
        }

        void Finish(bool success)
        {
            running = false;
            input.OnConfirmPressed.Unsubscribe(HandleConfirm);
            gameObject.SetActive(false);
            onResult?.Invoke(success);
        }
    }
}
