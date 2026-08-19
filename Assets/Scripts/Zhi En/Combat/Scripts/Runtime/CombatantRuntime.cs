using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// a live combatant for the duration of one battle - wraps whichever loadout/data SO it
    /// came from (player / partner / enemy) into one common shape the CombatManager can use
    /// without caring which side it's on. (I had to consult big gpt on how to make this work so TODO go rework it yourself like a human being)
    /// </summary>
    public class CombatantRuntime
    {
        public ActorType actorType;
        public string displayName;

        public int currentHP, maxHP;
        public int currentCS, maxCS; // enemies just won't use CS
        public int damage, defense;

        public bool isDefending;      // set true for the resolve step right after choosing Defend
        public bool isInvincible;     // set by an item/ability effect for one enemy turn
        public bool isAlive => currentHP > 0;

        /// <summary>where in the world this combatant's sprite/placeholder shape lives - used to anchor the menu icons, target arrow, etc.</summary>
        public Transform anchor;

        // -- source data, only one of these three will be non-null --
        public PlayerLoadoutSO playerSource;
        public PartnerLoadoutSO partnerSource;
        public EnemyDataSO enemySource;
        public int enemyMoveIndex; // where this enemy is in its hardcoded move order

        public static CombatantRuntime FromPlayer(PlayerLoadoutSO source, Transform anchor)
        {
            return new CombatantRuntime
            {
                actorType = ActorType.PLAYER,
                displayName = "Player", // TODO: pull an actual player name if you have one
                maxHP = source.maxHP,
                currentHP = source.maxHP,
                maxCS = source.maxCS,
                currentCS = 0,
                damage = source.TotalDamage(),
                defense = source.TotalDefense(),
                anchor = anchor,
                playerSource = source
            };
        }

        public static CombatantRuntime FromPartner(PartnerLoadoutSO source, Transform anchor)
        {
            return new CombatantRuntime
            {
                actorType = ActorType.PARTNER,
                displayName = source.partnerName,
                maxHP = source.maxHP,
                currentHP = source.maxHP,
                maxCS = source.maxCS,
                currentCS = 0,
                damage = source.TotalDamage(),
                defense = source.TotalDefense(),
                anchor = anchor,
                partnerSource = source
            };
        }

        public static CombatantRuntime FromEnemy(EnemyDataSO source, Transform anchor)
        {
            return new CombatantRuntime
            {
                actorType = ActorType.ENEMY,
                displayName = source.enemyName,
                maxHP = source.maxHP,
                currentHP = source.maxHP,
                maxCS = 0,
                currentCS = 0,
                damage = source.damage,
                defense = source.defense,
                anchor = anchor,
                enemySource = source
            };
        }

        /// <summary>applies incoming damage, respecting defense and the Defend stance. Returns the actual damage dealt (min 0).</summary>
        public int ApplyDamage(int incomingDamage)
        {
            if (isInvincible)
            {
                isInvincible = false; // one-hit shield, per design doc's "makes you invincible" item
                return 0;
            }

            int mitigated = incomingDamage - defense;
            if (isDefending)
                mitigated = Mathf.RoundToInt(mitigated * 0.5f); // TODO: tune the Defend damage-reduction ratio

            mitigated = Mathf.Max(mitigated, 0);
            currentHP = Mathf.Max(currentHP - mitigated, 0);
            return mitigated;
        }

        public void Heal(int amount) => currentHP = Mathf.Min(currentHP + amount, maxHP);
        public void GainCurse(int amount) => currentCS = Mathf.Min(currentCS + amount, maxCS);
        public void SpendCurse(int amount) => currentCS = Mathf.Max(currentCS - amount, 0);
    }
}
