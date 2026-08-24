using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Combat
{
    public class CombatantRuntime
    {
        public ActorType actorType;
        public string displayName;
        public Sprite combatSprite;

        public int currentHP, maxHP;
        public int currentCS, maxCS;
        public int damage, defense;

        public bool isDefending;
        public bool isAlive => currentHP > 0;

        public List<StatusEffectInstance> activeStatuses = new();

        public Transform anchor;

        public PlayerLoadoutSO playerSource;
        public PartnerLoadoutSO partnerSource;
        public EnemyDataSO enemySource;
        public int enemyMoveIndex;

        public static CombatantRuntime FromPlayer(PlayerLoadoutSO source, Transform anchor, Sprite sprite)
        {
            var playerBattleData = GameManager.CurrentUserData.PlayerBattleData;
            playerBattleData.Refresh();

            return new CombatantRuntime
            {
                actorType = ActorType.PLAYER,
                displayName = "Player",
                combatSprite = sprite,
                maxHP = playerBattleData.MaxHP,
                currentHP = playerBattleData.CurrentHP,
                maxCS = playerBattleData.MaxCurse,
                currentCS = playerBattleData.CurrentCurse,
                damage = 1 + source.TotalExtraDamage(),
                defense = source.TotalExtraDefense(),
                anchor = anchor,
                playerSource = source
            };
        }

        public static CombatantRuntime FromPartner(PlayerLoadoutSO playerSource, PartnerLoadoutSO source, Transform anchor, Sprite sprite)
        {
            return new CombatantRuntime
            {
                actorType = ActorType.PARTNER,
                displayName = source.partnerName,
                combatSprite = sprite,
                maxHP = source.maxHP,
                currentHP = source.maxHP,
                maxCS = source.maxCS,
                currentCS = 0,
                damage = source.baseDamage + playerSource.equippedGem.ExtraDamage,
                anchor = anchor,
                partnerSource = source
            };
        }

        public static CombatantRuntime FromEnemy(EnemyDataSO source, Transform anchor, Sprite sprite)
        {
            return new CombatantRuntime
            {
                actorType = ActorType.ENEMY,
                displayName = source.enemyName,
                combatSprite = sprite,
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

        public int ApplyDamage(int incomingDamage)
        {
            if (HasStatus(StatusEffectType.INVINCIBILITY))
                return 0;

            int mitigated = incomingDamage - defense;
            if (isDefending)
                mitigated = Mathf.RoundToInt(mitigated * 0.5f);

            mitigated = incomingDamage > 0 ? Mathf.Max(mitigated, 1) : Mathf.Max(mitigated, 0);

            currentHP = Mathf.Max(currentHP - mitigated, 0);
            return mitigated;
        }

        public int TakePoisonDamage(int amount)
        {
            int applied = Mathf.Max(amount, 1);
            currentHP = Mathf.Max(currentHP - applied, 0);
            return applied;
        }

        public void Heal(int amount) => currentHP = Mathf.Min(currentHP + amount, maxHP);
        public void GainCurse(int amount) => currentCS = Mathf.Min(currentCS + amount, maxCS);
        public void SpendCurse(int amount) => currentCS = Mathf.Max(currentCS - amount, 0);

        public bool HasStatus(StatusEffectType type) => activeStatuses.Any(s => s.type == type);

        public StatusEffectInstance GetStatus(StatusEffectType type) => activeStatuses.FirstOrDefault(s => s.type == type);

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