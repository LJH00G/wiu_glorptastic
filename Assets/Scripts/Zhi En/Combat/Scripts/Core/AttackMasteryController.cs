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
    ///
    /// The window is positioned/sized directly in pixels every attempt (anchoredPosition +
    /// sizeDelta), not via anchorMin/anchorMax fractions - anchors only render correctly if the
    /// element's Left/Right offsets happen to be exactly zero, and any stray offset left over
    /// from editing it by hand in the inspector made the visible box drift away from wherever
    /// the hit-test actually was. Driving it in pixels removes that dependency entirely.
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

            // force a known, unambiguous anchor/pivot setup once, so this component never
            // depends on however these were last left in the inspector.
            if (windowImage)
            {
                var rt = windowImage.rectTransform;
                rt.anchorMin = new Vector2(0f, 0f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 0.5f);
            }
            if (line)
            {
                line.anchorMin = new Vector2(0f, 0.5f);
                line.anchorMax = new Vector2(0f, 0.5f);
                line.pivot = new Vector2(0f, 0.5f);
            }
        }

        /// <param name="windowWidth01">0-1 fraction of the bar the green window covers - widen this via AccessorySO.masteryWindowWidthMultiplier</param>
        public void BeginAttempt(float windowWidth01, Action<bool> resultCallback)
        {
            onResult = resultCallback;
            windowWidth01 = Mathf.Clamp01(windowWidth01);

            windowStart01 = UnityEngine.Random.Range(0f, 1f - windowWidth01);
            windowEnd01 = windowStart01 + windowWidth01;

            timer = 0f;
            hitRegistered = false;
            running = true;
            gameObject.SetActive(true);

            PositionWindow();
            if (line) line.anchoredPosition = new Vector2(0f, line.anchoredPosition.y);

            input.OnConfirmPressed.Subscribe(HandleConfirm);
        }

        void PositionWindow()
        {
            if (!windowImage || !bar) return;

            float barWidth = bar.rect.width;
            var rt = windowImage.rectTransform;
            rt.anchoredPosition = new Vector2(windowStart01 * barWidth, 0f);
            rt.sizeDelta = new Vector2((windowEnd01 - windowStart01) * barWidth, 0f);
        }

        void HandleConfirm()
        {
            if (!running || hitRegistered) return;
            hitRegistered = true;

            float t01 = Mathf.Clamp01(timer / sweepDuration);
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
