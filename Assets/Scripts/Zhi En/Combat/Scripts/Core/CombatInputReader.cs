using Game.CSEvent;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Combat
{
    public enum MenuDirection { UP, DOWN, LEFT, RIGHT }

    /// <summary>
    /// single place that reads the combat control scheme (Arrow Keys / Z / X) and fans it out as events, so menu code, target selection, the flee minigame etc. don't each poll the keyboard indiv.
    /// </summary>
    public class CombatInputReader : MonoBehaviour
    {
        public EventCS<MenuDirection> OnDirectionPressed = new();
        public EventCS OnConfirmPressed = new();  // Z
        public EventCS OnCancelPressed = new();   // X

        /// <summary>
        /// raised on every frame Z or X is held down - only the flee minigame and attack mastery window care about raw "was this key down this frame" spam, everything else should use the *Pressed events above.
        /// </summary>
        public EventCS<bool> OnConfirmOrCancelHeldThisFrame = new(); // true = Z, false = X, only fires on the frame it's newly pressed

        /// <summary>combat input is only read while this is true - CombatManager toggles it off during animations/resolve steps if needed</summary>
        public bool InputEnabled { get; set; } = true;

        void Update()
        {
            if (!InputEnabled || Keyboard.current == null)
                return;

            var kb = Keyboard.current;

            if (kb.upArrowKey.wasPressedThisFrame) OnDirectionPressed.Raise(MenuDirection.UP);
            if (kb.downArrowKey.wasPressedThisFrame) OnDirectionPressed.Raise(MenuDirection.DOWN);
            if (kb.leftArrowKey.wasPressedThisFrame) OnDirectionPressed.Raise(MenuDirection.LEFT);
            if (kb.rightArrowKey.wasPressedThisFrame) OnDirectionPressed.Raise(MenuDirection.RIGHT);

            if (kb.zKey.wasPressedThisFrame)
            {
                OnConfirmPressed.Raise();
                OnConfirmOrCancelHeldThisFrame.Raise(true);
            }
            if (kb.xKey.wasPressedThisFrame)
            {
                OnCancelPressed.Raise();
                OnConfirmOrCancelHeldThisFrame.Raise(false);
            }
        }
    }
}
