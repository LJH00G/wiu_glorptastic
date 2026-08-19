namespace Game.Combat
{
    /// <summary>top level icons shown above a combatant's head (sword / star / bag / boot)</summary>
    public enum CombatMenuOption : byte
    {
        COMBAT,
        ACTIONS,
        ITEMS,
        FLEE
    }

    /// <summary>options inside the "Combat" textbox</summary>
    public enum CombatSubOption : byte
    {
        ATTACK,
        DEFEND
    }

    /// <summary>who a move can be aimed at</summary>
    public enum CombatTargetType : byte
    {
        SINGLE_ENEMY,
        ALL_ENEMIES,
        SELF,
        SELF_AND_PARTNER,
        SINGLE_ALLY
    }

    /// <summary>which side/slot a runtime combatant occupies</summary>
    public enum ActorType : byte
    {
        PLAYER,
        PARTNER,
        ENEMY
    }

    /// <summary>what an item / ability actually does when used</summary>
    public enum EffectType : byte
    {
        DAMAGE,
        HEAL_HP,
        HEAL_CS,
        CURE_STATUS,
        BUFF_DAMAGE,
        BUFF_DEFENSE,
        INVINCIBLE
    }

    /// <summary>a move an enemy can perform, chosen from its hardcoded move list</summary>
    public enum EnemyMoveType : byte
    {
        ATTACK,
        ABILITY
    }

    /// <summary>the overall combat state machine, driven by <see cref="CombatManager"/></summary>
    public enum CombatState : byte
    {
        TURN_START,
        MAIN_MENU,
        SUB_MENU,
        TARGET_SELECT,
        ATTACK_MASTERY,
        RESOLVE_ACTION,
        FLEE_MINIGAME,
        ENEMY_TURN,
        BATTLE_WON,
        BATTLE_LOST,
        FLED
    }
}
