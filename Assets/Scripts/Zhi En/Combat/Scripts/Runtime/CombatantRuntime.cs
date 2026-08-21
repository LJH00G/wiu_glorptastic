using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// a live combatant for the duration of one battle - wraps whichever loadout/data SO it
    /// came from (player / partner / enemy) into one common shape the CombatManager can drive
    /// without caring which side it's on.
    /// </summary>
    public class CombatantRuntime
    {
        public ActorType actorType;
        public string displayName;

        public int currentHP, maxHP;
        public int currentCS, maxCS; // enemies just won't use CS
        public int damage, defense;

        public bool isDefending;      // set true for the resolve step right after choosing Defend
        public bool isAlive => currentHP > 0;

        /// <summary>currently active status effects (Poison/Invincibility/Stun/...) - ticked down once per this combatant's own turn</summary>
        public List<StatusEffectInstance> activeStatuses = new();

        /// <summary>where in the world this combatant's sprite/placeholder shape lives - used to anchor the menu icons, target arrow, health bar, status icons, etc.</summary>
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

        /// <summary>applies incoming damage, respecting defense, the Defend stance, and Invincibility. Returns the actual damage dealt (min 0).</summary>
        public int ApplyDamage(int incomingDamage)
        {
            if (HasStatus(StatusEffectType.INVINCIBILITY))
                return 0; // blocks every hit for as long as the status is active, not just the first one

            int mitigated = incomingDamage - defense;
            if (isDefending)
                mitigated = Mathf.RoundToInt(mitigated * 0.5f); // TODO: tune the Defend damage-reduction ratio

            /* minimum damage floor - a real attack (incomingDamage > 0) always deals at least 1, even if defense would otherwise reduce it to 0 or below.
            Non-damage calls (0 or negative incoming) stay clamped at 0 so heals/etc. never route through here oddly. */
            mitigated = incomingDamage > 0 ? Mathf.Max(mitigated, 1) : Mathf.Max(mitigated, 0);

            currentHP = Mathf.Max(currentHP - mitigated, 0);
            return mitigated;
        }

        /// <summary>Poison damage bypasses defense entirely and is applied directly - still floored at 1 like any other real hit.</summary>
        public int TakePoisonDamage(int amount)
        {
            int applied = Mathf.Max(amount, 1);
            currentHP = Mathf.Max(currentHP - applied, 0);
            return applied;
        }

        public void Heal(int amount) => currentHP = Mathf.Min(currentHP + amount, maxHP);
        public void GainCurse(int amount) => currentCS = Mathf.Min(currentCS + amount, maxCS);
        public void SpendCurse(int amount) => currentCS = Mathf.Max(currentCS - amount, 0);

        //status effects

        public bool HasStatus(StatusEffectType type) => activeStatuses.Any(s => s.type == type);

        public StatusEffectInstance GetStatus(StatusEffectType type) => activeStatuses.FirstOrDefault(s => s.type == type);

        /// <summary>applies (or refreshes) a status effect. If already active, extends to whichever duration is longer rather than stacking - TODO: revisit if you want true stacking instead.</summary>
        public void ApplyStatus(StatusEffectType type, int duration, int power = 0)
        {
            Debug.Log($"{displayName}: Applying status {type}, duration={duration}, power={power}");

            var existing = GetStatus(type);
            if (existing != null)
            {
                existing.remainingTurns = Mathf.Max(existing.remainingTurns, duration);
                existing.power = power;
            }
            else
            {
                activeStatuses.Add(new StatusEffectInstance { type = type, remainingTurns = duration, power = power });
            }
        }

        public void CureStatus(StatusEffectType type) => activeStatuses.RemoveAll(s => s.type == type);

        public void CureAllStatuses() => activeStatuses.Clear();

        /// <summary>
        /// call once at the start of this combatant's own turn/action slot - ticks every active
        /// status down by 1 and removes any that just expired, invoking <paramref name="onExpired"/>
        /// for each one that wore off (useful for a "Poison wore off" style message).
        /// </summary>
        public void TickStatusesStartOfTurn(System.Action<StatusEffectType> onExpired)
        {
            for (int i = activeStatuses.Count - 1; i >= 0; i--)
            {
                activeStatuses[i].remainingTurns--;
                if (activeStatuses[i].remainingTurns <= 0)
                {
                    onExpired?.Invoke(activeStatuses[i].type);
                    activeStatuses.RemoveAt(i);
                }
            }
        }
    }
}