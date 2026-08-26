using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// once a move/item/ability is chosen, this decides who it can hit and lets the player arrow-key between candidates
    /// (for SINGLE_ENEMY / SINGLE_ALLY), or just confirms immediately for fixed-target types (ALL_ENEMIES / SELF / SELF_AND_PARTNER).
    /// </summary>
    public class TargetSelector : MonoBehaviour
    {
        CombatInputReader input;
        CombatHUD hud;

        List<CombatantRuntime> candidates = new();
        int cursor;
        Action<List<CombatantRuntime>> onConfirmed;
        Action onCancelled;
        bool listening;

        public void Init(CombatInputReader inputReader, CombatHUD combatHUD)
        {
            input = inputReader;
            hud = combatHUD;
            input.OnDirectionPressed.Subscribe(HandleDirection);
            input.OnConfirmPressed.Subscribe(HandleConfirm);
            input.OnCancelPressed.Subscribe(HandleCancel);
        }

        void OnDestroy()
        {
            if (input == null) return;
            input.OnDirectionPressed.Unsubscribe(HandleDirection);
            input.OnConfirmPressed.Unsubscribe(HandleConfirm);
            input.OnCancelPressed.Unsubscribe(HandleCancel);
        }

        /// <summary>
        /// starts target selection. For fixed-target types this resolves instantly (arrow still shows, but there's nothing to navigate).
        /// </summary>
        public void BeginSelection(CombatTargetType targetType, CombatantRuntime user,
            List<CombatantRuntime> allies, List<CombatantRuntime> enemies,
            Action<List<CombatantRuntime>> onTargetsConfirmed, Action onCancel)
        {
            onConfirmed = onTargetsConfirmed;
            onCancelled = onCancel;
            cursor = 0;

            switch (targetType)
            {
                case CombatTargetType.SINGLE_ENEMY:
                    candidates = enemies.Where(e => e.isAlive).ToList();
                    listening = candidates.Count > 0;
                    ShowArrowAtCursor();
                    if (candidates.Count == 0) onCancelled?.Invoke();
                    break;

                case CombatTargetType.SINGLE_ALLY:
                    candidates = allies.Where(a => a.isAlive).ToList();
                    listening = candidates.Count > 0;
                    ShowArrowAtCursor();
                    if (candidates.Count == 0) onCancelled?.Invoke();
                    break;

                case CombatTargetType.ALL_ENEMIES:
                    candidates = enemies.Where(e => e.isAlive).ToList();
                    hud.ShowTargetArrows(candidates.Select(c => c.anchor));
                    listening = false;
                    onConfirmed?.Invoke(candidates);
                    break;

                case CombatTargetType.SELF:
                    candidates = new List<CombatantRuntime> { user };
                    hud.ShowTargetArrows(candidates.Select(c => c.anchor));
                    listening = false;
                    onConfirmed?.Invoke(candidates);
                    break;

                case CombatTargetType.SELF_AND_PARTNER:
                    candidates = allies.Where(a => a.isAlive).ToList();
                    hud.ShowTargetArrows(candidates.Select(c => c.anchor));
                    listening = false;
                    onConfirmed?.Invoke(candidates);
                    break;
            }
        }

        void HandleDirection(MenuDirection dir)
        {
            if (!listening || candidates.Count == 0) return;
            if (dir == MenuDirection.LEFT) cursor = (cursor - 1 + candidates.Count) % candidates.Count;
            else if (dir == MenuDirection.RIGHT) cursor = (cursor + 1) % candidates.Count;
            ShowArrowAtCursor();
        }

        void HandleConfirm()
        {
            if (!listening) return;
            listening = false;
            onConfirmed?.Invoke(new List<CombatantRuntime> { candidates[cursor] });
        }

        void HandleCancel()
        {
            if (!listening) return;
            listening = false;
            hud.ClearTargetArrows();
            onCancelled?.Invoke();
        }

        void ShowArrowAtCursor()
        {
            if (candidates.Count == 0) return;
            hud.ShowTargetArrows(new[] { candidates[cursor].anchor });
        }
    }
}
