using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Combat
{
    /// <summary>
    /// top-left HP/CS numeric display, bottom-centre description textbox, and the downward
    /// arrow(s) that hover over whichever combatant(s) are currently targeted.
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

        [Header("Enemy Health Bars")]
        [SerializeField] EnemyHealthBarWidget enemyHealthBarPrefab;
        [SerializeField] Transform enemyHealthBarPool;
        readonly Dictionary<CombatantRuntime, EnemyHealthBarWidget> enemyHealthBars = new();

        [Header("Status Effect Icons")]
        [Tooltip("a small root GameObject with a HorizontalLayoutGroup on it - one is spawned under every combatant (player/partner/each enemy)")]
        [SerializeField] RectTransform statusIconRowPrefab;
        [Tooltip("a single plain Image - instantiated once per active status inside a combatant's row")]
        [SerializeField] Image statusIconPrefab;
        [SerializeField] Transform statusIconPool;
        [SerializeField] StatusEffectIconLibrarySO statusIconLibrary;
        readonly Dictionary<CombatantRuntime, RectTransform> statusIconRows = new();

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
        /// converts a world-space point (e.g. a combatant's anchor) into screen space and applies
        /// it to a UI RectTransform. Works with the default Screen Space - Overlay canvas as long
        /// as your scene camera is tagged "MainCamera" - no special canvas setup needed.
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

        /// <summary>spawns one health bar below each given enemy - call once when a battle starts</summary>
        public void SetupEnemyHealthBars(IEnumerable<CombatantRuntime> enemyList)
        {
            ClearEnemyHealthBars();
            if (!enemyHealthBarPrefab) return;

            foreach (var enemy in enemyList)
            {
                if (enemy == null || !enemy.anchor) continue;

                var widget = Instantiate(enemyHealthBarPrefab, enemyHealthBarPool ? enemyHealthBarPool : transform);
                widget.gameObject.SetActive(true);
                PositionAboveWorldPoint(widget.GetComponent<RectTransform>(), enemy.anchor.position, -1f); // negative offset = below the enemy
                widget.SetValue(enemy.currentHP, enemy.maxHP);
                enemyHealthBars[enemy] = widget;
            }
        }

        /// <summary>call whenever an enemy's HP might have changed (damage, heal, etc.)</summary>
        public void UpdateEnemyHealthBar(CombatantRuntime enemy)
        {
            if (enemy != null && enemyHealthBars.TryGetValue(enemy, out var widget) && widget)
                widget.SetValue(enemy.currentHP, enemy.maxHP);
        }

        public void ClearEnemyHealthBars()
        {
            foreach (var kv in enemyHealthBars)
                if (kv.Value) Destroy(kv.Value.gameObject);
            enemyHealthBars.Clear();
        }

        /// <summary>spawns an empty icon row below the given combatant - call once per combatant (player, partner, each enemy) when a battle starts</summary>
        public void SetupStatusIconRow(CombatantRuntime combatant)
        {
            if (combatant == null || !statusIconRowPrefab || !combatant.anchor) return;

            var row = Instantiate(statusIconRowPrefab, statusIconPool ? statusIconPool : transform);
            row.gameObject.SetActive(true);
            PositionAboveWorldPoint(row, combatant.anchor.position, -1.4f); // TODO: tune this offset so it sits below the health bar (enemies) or sprite (allies) rather than overlapping it
            statusIconRows[combatant] = row;
        }

        /// <summary>call whenever a combatant's active statuses change (applied, cured, or ticked/expired)</summary>
        public void RefreshStatusIcons(CombatantRuntime combatant)
        {
            if (combatant == null || !statusIconRows.TryGetValue(combatant, out var row) || !row) return;

            for (int i = row.childCount - 1; i >= 0; i--)
                Destroy(row.GetChild(i).gameObject);

            if (!statusIconPrefab) return;

            foreach (var status in combatant.activeStatuses)
            {
                var icon = Instantiate(statusIconPrefab, row);
                icon.gameObject.SetActive(true);
                if (statusIconLibrary)
                {
                    icon.sprite = statusIconLibrary.GetIcon(status.type);
                    icon.color = statusIconLibrary.GetColor(status.type);
                }
            }
        }

        public void ClearAllStatusIconRows()
        {
            foreach (var kv in statusIconRows)
                if (kv.Value) Destroy(kv.Value.gameObject);
            statusIconRows.Clear();
        }
    }
}