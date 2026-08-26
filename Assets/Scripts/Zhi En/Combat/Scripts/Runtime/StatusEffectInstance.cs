namespace Game.Combat
{
    /// <summary>
    /// one active status effect on a combatant. Ticked down by exactly 1 every time that
    /// combatant reaches the start of their own turn (see CombatManager.ProcessStartOfTurnStatuses),
    /// so "duration" means "lasts for N of the owner's own turns", not N full rounds.
    /// </summary>
    [System.Serializable]
    public class StatusEffectInstance
    {
        public StatusEffectType type;
        public int remainingTurns;

        /// <summary>meaning depends on type - currently only POISON uses this, as the damage dealt per turn tick</summary>
        public int power;
    }
}
