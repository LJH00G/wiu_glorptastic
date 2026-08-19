using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Combat
{
    /// <summary>
    /// the row of option icons that floats above a combatant's head (Combat/Actions/Items/Flee), and the small textbox that replaces it once one icon is chosen. Pure UI/navigation, CombatManager decides what the options actually do.
    /// </summary>
    public class CombatMenuUI : MonoBehaviour
    {
        [Header("Icon row")]
        [SerializeField] GameObject iconRowRoot;
        [SerializeField] Image[] icons; // assign in the order: Combat, Actions, Items, Flee
        [SerializeField] Color normalColor = Color.white;
        [SerializeField] Color highlightColor = Color.yellow;

        [Header("Submenu textbox")]
        [SerializeField] GameObject submenuRoot;
        [SerializeField] TextMeshProUGUI[] submenuLabels; // pre-allocate 6 and hide unused ones (TODO can reduce later maybe)

        CombatInputReader input;

        List<int> activeIconIndices = new();  // indices into icons that are currently selectable (partner has fewer than player)
        int iconCursor;

        List<string> currentSubOptions = new();
        int subCursor;

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

            iconCursor = 0;
            onIconConfirmed = onConfirmed;
            onCancelled = onCancel;

            if (submenuRoot) submenuRoot.SetActive(false);
            if (iconRowRoot) iconRowRoot.SetActive(true);

            if (anchor) CombatHUD.PositionAboveWorldPoint(iconRowRoot.GetComponent<RectTransform>(), anchor.position, 1.5f);

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
            onSubOptionConfirmed = onConfirmed;
            onCancelled = onCancel;

            if (iconRowRoot) iconRowRoot.SetActive(false);
            if (submenuRoot) submenuRoot.SetActive(true);

            if (anchor) CombatHUD.PositionAboveWorldPoint(submenuRoot.GetComponent<RectTransform>(), anchor.position, 1.5f);

            for (int i = 0; i < submenuLabels.Length; i++)
            {
                bool inUse = i < options.Count;
                submenuLabels[i].gameObject.SetActive(inUse);
                if (inUse) submenuLabels[i].text = options[i];
            }

            RefreshSubHighlight();
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
                RefreshSubHighlight();
            }
        }

        void HandleConfirm()
        {
            if (listeningIconRow)
                onIconConfirmed?.Invoke(activeIconIndices[iconCursor]);
            else if (listeningSubmenu)
                onSubOptionConfirmed?.Invoke(subCursor);
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

        void RefreshSubHighlight()
        {
            for (int i = 0; i < currentSubOptions.Count; i++)
                submenuLabels[i].color = (i == subCursor) ? highlightColor : normalColor;
        }

        static int Wrap(int value, int count) => count <= 0 ? 0 : (value % count + count) % count;
    }
}
