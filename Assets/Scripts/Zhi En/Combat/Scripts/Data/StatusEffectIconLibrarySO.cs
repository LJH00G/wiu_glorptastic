using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// maps each StatusEffectType to a placeholder icon + tint, so Poison/Invincibility/Stun
    /// can look visually distinct under a combatant even while sharing the same generic sprite.
    /// Add entries here for Poison, Invincibility, and Stun (and any future status types).
    /// </summary>
    [CreateAssetMenu(menuName = "Combat/Status Effect Icon Library", fileName = "StatusEffectIcons")]
    public class StatusEffectIconLibrarySO : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public StatusEffectType type;
            public Sprite icon;
            public Color tint;
        }

        public List<Entry> entries = new();

        public Sprite GetIcon(StatusEffectType type)
        {
            foreach (var e in entries)
                if (e.type == type) return e.icon;
            return null;
        }

        public Color GetColor(StatusEffectType type)
        {
            foreach (var e in entries)
                if (e.type == type) return e.tint;
            return Color.white;
        }
    }
}
