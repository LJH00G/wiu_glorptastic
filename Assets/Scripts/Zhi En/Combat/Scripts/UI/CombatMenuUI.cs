using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Combat
{
    /// <summary>
    /// the row of option icons that floats above a combatant's head (Combat/Actions/Items/Flee),
    /// and the small textbox that replaces it once one icon is chosen.
    /// </summary>
    public class CombatMenuUI : MonoBehaviour
    {
        [Header("Icon row")]
        [SerializeField] GameObject iconRowRoot;
        [SerializeField] Image[] icons; // assign in the order: Combat, Actions, Items, Flee
        [SerializeField] Color normalColor = Color.white;
        [SerializeField] Color highlightColor = Color.yellow;
        [Tooltip("the IconRow's own HorizontalLayoutGroup - spacing shrinks automatically for shorter menus (e.g. the partner's 2-icon menu)")]
        [SerializeField] HorizontalLayoutGroup iconRowLayout;
        [SerializeField] float fullMenuSpacing = 20f;
        [SerializeField] float reducedMenuSpacing = 8f;

        [Header("Submenu textbox")]
        [SerializeField] GameObject submenuRoot;
        [SerializeField] TextMarkupTypeWriter[] submenuLabels; // pool size = how many rows are visible at once; list can be longer and will scroll

        CombatInputReader input;

        List<int> activeIconIndices = new();  // indices into `icons` that are currently selectable (partner has fewer than player)
        int iconCursor;

        List<string> currentSubOptions = new();
        int subCursor;
        int subScrollOffset; // index of the option currently shown in submenuLabels[0]

        Action<int> onIconConfirmed;   // returns index into activeIconIndices
        Action<int> onSubOptionConfirmed;
        Action onCancelled;

        bool listeningIconRow;
        bool listeningSubmenu;

        public void Init(CombatInputReader inputReader)
        {
            input = inputReader;
            input.OnDirectionPressed.Subscribe(HandleDirection);
            input.OnConfirmPressed.Subscribe(HandleConfirm);
            input.OnCancelPressed.Subscribe(HandleCancel);

            if (iconRowRoot) iconRowRoot.SetActive(false);
            if (submenuRoot) submenuRoot.SetActive(false);
        }

        void OnDestroy()
        {
            if (input == null) return;
            input.OnDirectionPressed.Unsubscribe(HandleDirection);
            input.OnConfirmPressed.Unsubscribe(HandleConfirm);
            input.OnCancelPressed.Unsubscribe(HandleCancel);
        }

        /// <param name="availableOptions">which of the 4 icons this combatant is allowed to use - player gets all 4, partner only Combat+Actions</param>
        /// <param name="anchor">world-space transform to hover this menu above (the acting combatant's anchor)</param>
        public void ShowIconRow(List<CombatMenuOption> availableOptions, Transform anchor, Action<int> onConfirmed, Action onCancel)
        {
            activeIconIndices.Clear();
            foreach (var opt in availableOptions)
                activeIconIndices.Add((int)opt);

            /* keep whatever was highlighted before(e.g.after cancelling out of a submenu)
            only snap back to the first option if the old cursor no longer fits this menu
            (different actor/different option count) */
            if (iconCursor < 0 || iconCursor >= activeIconIndices.Count)
                iconCursor = 0;

            onIconConfirmed = onConfirmed;
            onCancelled = onCancel;

            if (submenuRoot) submenuRoot.SetActive(false);
            if (iconRowRoot) iconRowRoot.SetActive(true);

            if (anchor) CombatHUD.PositionAboveWorldPoint(iconRowRoot.GetComponent<RectTransform>(), anchor.position, 2.5f);

            if (iconRowLayout) iconRowLayout.spacing = activeIconIndices.Count <= 2 ? reducedMenuSpacing : fullMenuSpacing;

            for (int i = 0; i < icons.Length; i++)
                if (icons[i]) icons[i].gameObject.SetActive(activeIconIndices.Contains(i));

            RefreshIconHighlight();
            listeningIconRow = true;
            listeningSubmenu = false;
        }

        public void ShowSubmenu(List<string> options, Transform anchor, Action<int> onConfirmed, Action onCancel)
        {
            currentSubOptions = options;
            subCursor = 0;
            subScrollOffset = 0;
            onSubOptionConfirmed = onConfirmed;
            onCancelled = onCancel;

            if (iconRowRoot) iconRowRoot.SetActive(false);
            if (submenuRoot) submenuRoot.SetActive(true);

            if (anchor) CombatHUD.PositionAboveWorldPoint(submenuRoot.GetComponent<RectTransform>(), anchor.position, 1.5f);

            RefreshSubmenuDisplay();
            listeningIconRow = false;
            listeningSubmenu = true;
        }

        public void HideAll()
        {
            if (iconRowRoot) iconRowRoot.SetActive(false);
            if (submenuRoot) submenuRoot.SetActive(false);
            listeningIconRow = listeningSubmenu = false;
        }

        void HandleDirection(MenuDirection dir)
        {
            if (listeningIconRow)
            {
                if (dir == MenuDirection.LEFT) iconCursor = Wrap(iconCursor - 1, activeIconIndices.Count);
                else if (dir == MenuDirection.RIGHT) iconCursor = Wrap(iconCursor + 1, activeIconIndices.Count);
                RefreshIconHighlight();
            }
            else if (listeningSubmenu)
            {
                if (dir == MenuDirection.UP) subCursor = Wrap(subCursor - 1, currentSubOptions.Count);
                else if (dir == MenuDirection.DOWN) subCursor = Wrap(subCursor + 1, currentSubOptions.Count);
                RefreshSubmenuDisplay();
            }
        }

        void HandleConfirm()
        {
            if (listeningIconRow)
                onIconConfirmed?.Invoke(activeIconIndices[iconCursor]);
            else if (listeningSubmenu)
                onSubOptionConfirmed?.Invoke(subCursor); // still an absolute index into currentSubOptions - callers are unaffected by scrolling
        }

        void HandleCancel()
        {
            if (listeningIconRow || listeningSubmenu)
                onCancelled?.Invoke();
        }

        void RefreshIconHighlight()
        {
            for (int i = 0; i < icons.Length; i++)
                if (icons[i]) icons[i].color = normalColor;
            if (activeIconIndices.Count > 0)
                icons[activeIconIndices[iconCursor]].color = highlightColor;
        }

        /// <summary>
        /// Fills the fixed-size submenuLabels pool with a "window" of currentSubOptions, scrolling
        /// that window so subCursor is always kept visible. Replaces the old RefreshSubHighlight -
        /// that version indexed submenuLabels[i] for i up to currentSubOptions.Count, which would
        /// throw once options.Count exceeded submenuLabels.Length.
        /// </summary>
        void RefreshSubmenuDisplay()
        {
            int visibleCount = submenuLabels.Length;

            if (subCursor < subScrollOffset)
                subScrollOffset = subCursor;
            else if (subCursor >= subScrollOffset + visibleCount)
                subScrollOffset = subCursor - visibleCount + 1;

            subScrollOffset = Mathf.Clamp(subScrollOffset, 0, Mathf.Max(0, currentSubOptions.Count - visibleCount));

            for (int i = 0; i < submenuLabels.Length; i++)
            {
                int optionIndex = subScrollOffset + i;
                bool inUse = optionIndex < currentSubOptions.Count;

                submenuLabels[i].gameObject.SetActive(inUse);

                if (inUse)
                {
                    submenuLabels[i].StartNewTypeWriting((optionIndex == subCursor ? "> " : "") + currentSubOptions[optionIndex], true);
                }
            }
        }

        static int Wrap(int value, int count) => count <= 0 ? 0 : (value % count + count) % count;
    }
}