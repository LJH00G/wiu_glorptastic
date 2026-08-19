using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// top-left HP/CS numeric display, bottom-centre description textbox, and the indicator arrows (that are currently squares for some reason I'm not very smart I think)
    /// </summary>
    public class CombatHUD : MonoBehaviour
    {
        [Header("Top-left stat readout")]
        [SerializeField] TextMeshProUGUI hpText;
        [SerializeField] TextMeshProUGUI csText;

        [Header("Bottom-centre description box")]
        [SerializeField] GameObject descriptionBox;
        [SerializeField] TextMeshProUGUI descriptionText;

        [Header("Target arrow")]
        [Tooltip("simple downward-pointing triangle/arrow, parented under this canvas and repositioned above targets")]
        [SerializeField] RectTransform arrowPrefab;
        [SerializeField] Transform arrowPool;
        readonly List<RectTransform> activeArrows = new();

        void Awake()
        {
            if (descriptionBox) descriptionBox.SetActive(false);
        }

        public void UpdateStats(int hp, int maxHp, int cs, int maxCs)
        {
            if (hpText) hpText.text = $"Hp: {hp}/{maxHp}";
            if (csText) csText.text = $"Cs: {cs}/{maxCs}";
        }

        public void ShowDescription(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                HideDescription();
                return;
            }
            if (descriptionBox) descriptionBox.SetActive(true);
            if (descriptionText) descriptionText.text = text;
        }

        public void HideDescription()
        {
            if (descriptionBox) descriptionBox.SetActive(false);
        }

        /// <summary>points an arrow above every transform passed in (1 for a single target, 2+ for AoE/self+partner), clearing any previous arrows first</summary>
        public void ShowTargetArrows(IEnumerable<Transform> targets)
        {
            ClearTargetArrows();
            if (arrowPrefab == null) return;

            foreach (var t in targets)
            {
                if (!t) continue;
                RectTransform arrow = Instantiate(arrowPrefab, arrowPool ? arrowPool : transform);
                arrow.gameObject.SetActive(true);
                PositionAboveWorldPoint(arrow, t.position);
                activeArrows.Add(arrow);
            }
        }

        /// <summary>
        /// converts a world-space point (e.g. a combatant's anchor) into screen space and applies it to a UI RectTransform. Works with the default Screen Space - Overlay canvas as long as your scene camera is tagged "MainCamera" - no special canvas setup needed.
        /// </summary>
        public static void PositionAboveWorldPoint(RectTransform uiElement, Vector3 worldPoint, float heightOffset = 1f)
        {
            if (!uiElement || Camera.main == null) return;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPoint + Vector3.up * heightOffset);
            uiElement.position = screenPoint;
        }

        public void ClearTargetArrows()
        {
            foreach (var arrow in activeArrows)
                if (arrow) Destroy(arrow.gameObject);
            activeArrows.Clear();
        }
    }
}
