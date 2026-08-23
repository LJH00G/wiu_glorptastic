using Game.Combat.Integration;
using Game.SO.Data.Item.Sellable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Inventory;

namespace Game.Combat
{
    /// <summary>
    /// Drives the whole battle. Each round: Player acts, then Partner acts (if present and
    /// alive), then every alive enemy acts, then repeat. Choosing "Switch to Partner" from the
    /// player's Actions menu reorders JUST that round so the Partner goes first, then the
    /// Player - the next round goes back to the normal Player-then-Partner order automatically.
    ///
    /// Status effects (Poison/Invincibility/Stun) tick down once at the start of each
    /// combatant's own turn slot - see ProcessStartOfTurnStatuses().
    ///
    /// Ally HP/CS regeneration is applied once per ally round, before the first ally acts.
    /// This prevents Player and Partner regeneration from being applied multiple times
    /// during the same round.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Data")]
        public PlayerLoadoutSO playerLoadout;
        public PartnerLoadoutSO partnerLoadout; // leave empty if partner isn't unlocked yet
        public EnemyDataSO[] enemyEncounter;

        [Header("Anchors (world positions to spawn/point arrows at)")]
        [SerializeField] Transform playerAnchor;
        [SerializeField] Transform partnerAnchor;
        public Transform[] enemyAnchors;

        [Header("Wiring")]
        [SerializeField] CombatInputReader input;
        [SerializeField] CombatHUD hud;
        [SerializeField] CombatMenuUI menuUI;
        [SerializeField] TargetSelector targetSelector;
        [SerializeField] FleeMinigameController flee;
        [SerializeField] AttackMasteryController mastery;
        [SerializeField] CombatAssigmment combatAssigner;

        CombatState state;
        CombatantRuntime player;
        CombatantRuntime partner; // null if not brought into this battle
        List<CombatantRuntime> enemies = new();

        // This round's ally turn order.
        // Rebuilt fresh every round as [player, partner], and only reordered
        // mid-round by "Switch to Partner".
        List<CombatantRuntime> allyTurnOrder = new();
        int allyTurnIndex;

        List<CombatantRuntime> Allies => partner != null
            ? new List<CombatantRuntime> { player, partner }
            : new List<CombatantRuntime> { player };

        

        public void SetupBattle()
        {
            player = CombatantRuntime.FromPlayer(playerLoadout, playerAnchor);
            partner = partnerLoadout != null
                ? CombatantRuntime.FromPartner(playerLoadout, partnerLoadout, partnerAnchor)
                : null;

            enemies.Clear();

            for (int i = 0; i < enemyEncounter.Length; i++)
            {
                Transform anchor =
                    (enemyAnchors != null && i < enemyAnchors.Length)
                        ? enemyAnchors[i]
                        : null;

                enemies.Add(
                    CombatantRuntime.FromEnemy(enemyEncounter[i], anchor)
                );
            }

            // Keep the initialization order from the known-good version.
            menuUI.Init(input);
            targetSelector.Init(input, hud);
            flee.Init(input);
            mastery.Init(input);

            hud.SetupEnemyHealthBars(enemies);

            // Status icon rows are part of the newer status-effect system.
            hud.SetupStatusIconRow(player);

            if (partner != null)
                hud.SetupStatusIconRow(partner);

            foreach (var enemy in enemies)
                hud.SetupStatusIconRow(enemy);

            BeginAllyRound();
        }

        void BeginAllyRound()
        {
            player.isDefending = false;

            if (partner != null)
                partner.isDefending = false;

            hud.ClearTargetArrows();

            allyTurnOrder = new List<CombatantRuntime> { player };

            if (partner != null && partner.isAlive)
                allyTurnOrder.Add(partner);

            allyTurnIndex = 0;

            foreach (var actor in allyTurnOrder)
                ApplyStartOfTurnRegen(actor);

            hud.UpdateStats(
                player.currentHP,
                player.maxHP,
                player.currentCS,
                player.maxCS
            );

            BeginAllyTurn();
        }

        void BeginAllyTurn()
        {
            // Safety check in case something removed the actor from the battle between
            // turns.
            if (allyTurnIndex >= allyTurnOrder.Count)
            {
                BeginEnemyTurn();
                return;
            }

            var actor = allyTurnOrder[allyTurnIndex];

            bool skipTurn = ProcessStartOfTurnStatuses(actor);

            // Poison may have killed the actor before their turn begins.
            if (skipTurn || !actor.isAlive)
            {
                EndAllyAction();
                return;
            }

            state = CombatState.MAIN_MENU;

            ShowMenuFor(actor, isPlayer: actor == player);

            hud.UpdateStats(
                player.currentHP,
                player.maxHP,
                player.currentCS,
                player.maxCS
            );
        }

        void ApplyStartOfTurnRegen(CombatantRuntime actor)
        {
            if (actor.playerSource == null)
                return; // regen accessories are a player-only concept for now

            actor.Heal(actor.playerSource.TotalHPRegen());
            actor.GainCurse(actor.playerSource.TotalCSRegen());
        }

        /// <summary>
        /// Applies Poison damage and ticks every active status down by 1 for whoever is
        /// about to act.
        ///
        /// Returns true if this combatant is Stunned and should have their turn skipped
        /// entirely. The turn slot is still consumed.
        /// </summary>
        bool ProcessStartOfTurnStatuses(CombatantRuntime actor)
        {
            if (actor.HasStatus(StatusEffectType.POISON))
            {
                var poison = actor.GetStatus(StatusEffectType.POISON);

                int dealt = actor.TakePoisonDamage(poison.power);

                hud.ShowDescription(
                    $"{actor.displayName} takes {dealt} poison damage!"
                );

                hud.RefreshStatusIcons(actor);

                CheckDeaths();

                // Poison killed this combatant.
                if (!actor.isAlive)
                    return false;
            }

            bool stunned = actor.HasStatus(StatusEffectType.STUN);

            if (stunned)
            {
                hud.ShowDescription(
                    $"{actor.displayName} is stunned and can't move!"
                );
            }

            actor.TickStatusesStartOfTurn(
                expired =>
                    hud.ShowDescription(
                        $"{actor.displayName}'s {FriendlyStatusName(expired)} wore off."
                    )
            );

            hud.RefreshStatusIcons(actor);

            return stunned;
        }

        static string FriendlyStatusName(StatusEffectType type) => type switch
        {
            StatusEffectType.POISON => "poison",
            StatusEffectType.INVINCIBILITY => "invincibility",
            StatusEffectType.STUN => "stun",
            _ => type.ToString()
        };

        void ShowMenuFor(CombatantRuntime actor, bool isPlayer)
        {
            state = CombatState.MAIN_MENU;

            var options = isPlayer
                ? new List<CombatMenuOption>
                {
                    CombatMenuOption.COMBAT,
                    CombatMenuOption.ACTIONS,
                    CombatMenuOption.ITEMS,
                    CombatMenuOption.FLEE
                }
                : new List<CombatMenuOption>
                {
                    CombatMenuOption.COMBAT,
                    CombatMenuOption.ACTIONS
                };

            menuUI.ShowIconRow(
                options,
                actor.anchor,
                idx => OnMainMenuSelected(
                    actor,
                    isPlayer,
                    (CombatMenuOption)idx
                ),
                () => { }
            );
        }

        void OnMainMenuSelected(
            CombatantRuntime actor,
            bool isPlayer,
            CombatMenuOption option)
        {
            switch (option)
            {
                case CombatMenuOption.COMBAT:
                    state = CombatState.SUB_MENU;

                    menuUI.ShowSubmenu(
                        new List<string> { "Attack", "Defend" },
                        actor.anchor,
                        idx => OnCombatSubSelected(actor, idx),
                        () => ShowMenuFor(actor, isPlayer)
                    );
                    break;

                case CombatMenuOption.ACTIONS:
                    ShowActionsMenu(actor, isPlayer);
                    break;

                case CombatMenuOption.ITEMS:
                    ShowItemsMenu(actor, isPlayer);
                    break;

                case CombatMenuOption.FLEE:
                    AttemptFlee();
                    break;
            }
        }

        void ShowActionsMenu(CombatantRuntime actor, bool isPlayer)
        {
            // Only offer the swap while the partner hasn't acted yet this round.
            bool partnerStillPending =
                partner != null &&
                partner.isAlive &&
                allyTurnOrder.IndexOf(partner) > allyTurnIndex;

            bool showSwitch = isPlayer && partnerStillPending;

            List<AbilitySO> abilities =
                actor.playerSource != null
                    ? actor.playerSource.knownAbilities
                    : actor.partnerSource.knownAbilities;

            List<string> labels = new();

            if (showSwitch)
                labels.Add("Switch to Partner");

            foreach (var ability in abilities)
            {
                labels.Add(
                    $"{ability.abilityName} ({ability.curseCost} Cs)"
                );
            }

            state = CombatState.SUB_MENU;

            menuUI.ShowSubmenu(
                labels,
                actor.anchor,
                idx =>
                {
                    if (showSwitch && idx == 0)
                    {
                        // Swap only this round.
                        int partnerIndex = allyTurnOrder.IndexOf(partner);

                        (
                            allyTurnOrder[allyTurnIndex],
                            allyTurnOrder[partnerIndex]
                        ) =
                        (
                            allyTurnOrder[partnerIndex],
                            allyTurnOrder[allyTurnIndex]
                        );

                        hud.ShowDescription(
                            $"{player.displayName} lets {partner.displayName} go first!"
                        );

                        menuUI.HideAll();

                        // The partner is now sitting at allyTurnIndex.
                        BeginAllyTurn();
                        return;
                    }

                    int abilityIndex = showSwitch
                        ? idx - 1
                        : idx;

                    AbilitySO ability = abilities[abilityIndex];

                    OnAbilitySelected(actor, ability);
                },
                () => ShowMenuFor(actor, isPlayer)
            );
        }

        void OnAbilitySelected(
            CombatantRuntime actor,
            AbilitySO ability)
        {
            // IMPORTANT BUG FIX:
            // Partner abilities are paid for using the Player's Curse, not the
            // Partner's Curse.
            //
            // The lower, known-good CombatManager explicitly checked the Player's
            // Curse when the Partner used an ability. Preserve that behavior here.
            if (actor.actorType == ActorType.PARTNER)
            {
                foreach (var ally in Allies)
                {
                    if (ally.actorType == ActorType.PLAYER)
                    {
                        if (ally.currentCS < ability.curseCost)
                        {
                            hud.ShowDescription(
                                "Not enough curse energy!"
                            );

                            // Stay on the ability submenu.
                            return;
                        }

                        break;
                    }
                }
            }
            else if (actor.actorType == ActorType.PLAYER)
            {
                if (actor.currentCS < ability.curseCost)
                {
                    hud.ShowDescription(
                        "Not enough curse energy!"
                    );

                    // Stay on the ability submenu.
                    return;
                }
            }

            hud.ShowDescription(ability.description);

            // Stop the submenu from listening for Z/arrow keys before target
            // selection takes over.
            menuUI.HideAll();

            state = CombatState.TARGET_SELECT;

            targetSelector.BeginSelection(
                ability.targetType,
                actor,
                Allies,
                enemies,
                targets => ResolveAbility(actor, ability, targets),
                () => ShowActionsMenu(actor, actor == player)
            );
        }

        void ShowItemsMenu(CombatantRuntime actor, bool isPlayer)
        {
            // Items are player-only.
            if (actor.playerSource == null)
                return;

            var inventory = actor.playerSource.inventory;

            var usable = inventory.Select((stack, index) => (stack, index)).Where(pair => pair.stack.item is ConsumableItemSO).ToList();

            List<string> labels = inventory.Select(stack =>$"{stack.item.Name} x{stack.count}").ToList();

            state = CombatState.SUB_MENU;

            menuUI.ShowSubmenu(labels,actor.anchor, idx =>
                {
                    var stack = inventory.ElementAtOrDefault(idx);

                    if (stack.count <= 0)
                        return;

                    OnItemSelected(
                        actor,
                        idx,
                        (ConsumableItemSO)stack.item
                    );
                },
                () => ShowMenuFor(actor, isPlayer)
            );
        }

        void OnItemSelected(
            CombatantRuntime actor,
            int inventoryIndex,
            ConsumableItemSO item)
        {
            hud.ShowDescription(item.Description);

            menuUI.HideAll();

            state = CombatState.TARGET_SELECT;

            targetSelector.BeginSelection(
                item.TargetType,
                actor,
                Allies,
                enemies,
                targets =>
                {
                    ResolveItem(actor, item, targets);

                    if (item.ConsumeOnUse)
                    {
                        var stack =
                            actor.playerSource.inventory[inventoryIndex];

                        stack.count = stack.count > 0 ? stack.count - 1 : 0;

                        actor.playerSource.inventory[inventoryIndex] =
                            stack;
                    }
                },
                () => ShowItemsMenu(actor, true)
            );
        }

        void OnCombatSubSelected(
            CombatantRuntime actor,
            int idx)
        {
            var subOption = (CombatSubOption)idx;

            if (subOption == CombatSubOption.ATTACK)
            {
                menuUI.HideAll();

                state = CombatState.TARGET_SELECT;

                targetSelector.BeginSelection(
                    CombatTargetType.SINGLE_ENEMY,
                    actor,
                    Allies,
                    enemies,
                    targets =>
                        ResolveAttack(
                            actor,
                            targets[0]
                        ),
                    () =>
                        OnMainMenuSelected(
                            actor,
                            actor == player,
                            CombatMenuOption.COMBAT
                        )
                );
            }
            else
            {
                ResolveDefend(actor);
            }
        }


        void ResolveAttack(
            CombatantRuntime actor,
            CombatantRuntime target)
        {
            hud.ClearTargetArrows();

            // Attack mastery triggers for every ally attack, never enemies.
            if (actor != null && actor.actorType != ActorType.ENEMY)
            {
                float width =
                    0.25f *
                    playerLoadout.TotalMasteryWidthMultiplier();

                mastery.BeginAttempt(
                    width,
                    bonusHit =>
                        FinishAttack(
                            actor,
                            target,
                            bonusHit ? 1 : 0
                        )
                );
            }
            else
            {
                FinishAttack(actor, target, 0);
            }
        }

        void FinishAttack(
            CombatantRuntime actor,
            CombatantRuntime target,
            int masteryBonus)
        {
            int dealt =
                target.ApplyDamage(
                    actor.damage + masteryBonus
                );

            hud.ShowDescription(
                $"{actor.displayName} hits {target.displayName} for {dealt}!"
            );

            CheckDeaths();
            EndAllyAction();
        }

        void ResolveDefend(CombatantRuntime actor)
        {
            actor.isDefending = true;
            actor.GainCurse(5);

            hud.ShowDescription(
                $"{actor.displayName} braces for the next attack."
            );

            EndAllyAction();
        }

        void ResolveAbility(
            CombatantRuntime actor,
            AbilitySO ability,
            List<CombatantRuntime> targets)
        {
            hud.ClearTargetArrows();

            // IMPORTANT BUG FIX:
            // Partner abilities consume the Player's Curse.
            //
            // The ability is still performed by the Partner, so effects are
            // applied with the Partner as the source, but the resource belongs
            // to the Player.
            if (actor.actorType == ActorType.PARTNER)
            {
                foreach (var ally in Allies)
                {
                    if (ally.actorType == ActorType.PLAYER)
                    {
                        ally.SpendCurse(ability.curseCost);
                        break;
                    }
                }
            }
            else if (actor.actorType == ActorType.PLAYER)
            {
                actor.SpendCurse(ability.curseCost);
            }

            // New feature:
            // AbilitySO now contains multiple EffectEntry entries rather than
            // the old single effect/power pair.
            foreach (var target in targets)
            {
                foreach (var entry in ability.effects)
                {
                    ApplyEffect(
                        actor,
                        target,
                        entry
                    );
                }
            }

            hud.ShowDescription(
                $"{actor.displayName} uses {ability.abilityName}!"
            );

            CheckDeaths();
            EndAllyAction();
        }

        void ResolveItem(
            CombatantRuntime actor,
            ConsumableItemSO item,
            List<CombatantRuntime> targets)
        {
            hud.ClearTargetArrows();

            // New feature:
            // Items can contain multiple EffectEntry entries.
            foreach (var target in targets)
            {
                foreach (var entry in item.Effects)
                {
                    ApplyEffect(
                        actor,
                        target,
                        entry
                    );
                }
            }

            hud.ShowDescription(
                $"{actor.displayName} uses {item.Name}!"
            );

            CheckDeaths();
            EndAllyAction();
        }

        /// <summary>
        /// Applies one effect entry from an item's or ability's effect list
        /// to one target.
        /// </summary>
        void ApplyEffect(
            CombatantRuntime source,
            CombatantRuntime target,
            EffectEntry entry)
        {
            switch (entry.effect)
            {
                case EffectType.DAMAGE:
                    target.ApplyDamage(entry.power);
                    break;

                case EffectType.HEAL_HP:
                    target.Heal(entry.power);
                    break;

                case EffectType.HEAL_CS:
                    target.GainCurse(entry.power);
                    break;

                case EffectType.BUFF_DAMAGE:
                    target.damage += entry.power;
                    break;

                case EffectType.BUFF_DEFENSE:
                    target.defense += entry.power;
                    break;

                case EffectType.APPLY_STATUS:
                    target.ApplyStatus(
                        entry.status,
                        entry.duration,
                        entry.power
                    );
                    break;

                case EffectType.CURE_STATUS:
                    target.CureStatus(entry.status);
                    break;
            }

            hud.RefreshStatusIcons(target);
        }

        void AttemptFlee()
        {
            int combined =
                enemies
                    .Where(e => e.isAlive)
                    .Sum(e => e.damage + e.defense);

            flee.SetRequiredPressesFromEnemies(combined);

            state = CombatState.FLEE_MINIGAME;

            hud.ShowDescription(
                "Mash Z and X to break free!"
            );

            menuUI.HideAll();

            flee.BeginAttempt(success =>
            {
                if (success)
                {
                    state = CombatState.FLED;

                    hud.ShowDescription(
                        "Got away safely!"
                    );

                    EndBattle(won: false);
                }
                else
                {
                    hud.ShowDescription(
                        "Couldn't escape!"
                    );

                    // Failed flee consumes the entire party's turn.
                    SkipToEnemyTurn();
                }
            });
        }

        void EndAllyAction()
        {
            if (state == CombatState.BATTLE_WON ||
                state == CombatState.BATTLE_LOST ||
                state == CombatState.FLED)
            {
                return;
            }

            menuUI.HideAll();
            hud.ClearTargetArrows();

            allyTurnIndex++;

            if (allyTurnIndex < allyTurnOrder.Count)
            {
                BeginAllyTurn();
            }
            else
            {
                BeginEnemyTurn();
            }
        }

        void SkipToEnemyTurn()
        {
            if (state == CombatState.BATTLE_WON ||
                state == CombatState.BATTLE_LOST ||
                state == CombatState.FLED)
            {
                return;
            }

            menuUI.HideAll();
            hud.ClearTargetArrows();

            BeginEnemyTurn();
        }

        void BeginEnemyTurn()
        {
            state = CombatState.ENEMY_TURN;

            // TODO:
            // Once animations are available, resolve enemies through a coroutine
            // or DelayedCallbackInvoker so each enemy action has a visible pause.

            foreach (var enemy in enemies.ToList())
            {
                if (!enemy.isAlive)
                    continue;

                // New feature:
                // Enemy status effects process at the beginning of the enemy's
                // own turn slot.
                bool skipTurn =
                    ProcessStartOfTurnStatuses(enemy);

                // Poison may have killed the enemy.
                if (!enemy.isAlive)
                    continue;

                // Stun consumes the enemy's turn.
                if (skipTurn)
                    continue;

                var move =
                    EnemyFSMController.GetNextMove(enemy);

                ResolveEnemyMove(
                    enemy,
                    move
                );

                if (!player.isAlive &&
                    (partner == null || !partner.isAlive))
                {
                    break;
                }
            }

            CheckDeaths();

            if (state == CombatState.BATTLE_WON ||
                state == CombatState.BATTLE_LOST)
            {
                return;
            }

            BeginAllyRound();
        }

        void ResolveEnemyMove(
            CombatantRuntime enemy,
            EnemyMove move)
        {
            List<CombatantRuntime> targets =
                move.targetType switch
                {
                    CombatTargetType.SELF =>
                        new List<CombatantRuntime>
                        {
                            enemy
                        },

                    CombatTargetType.ALL_ENEMIES =>
                        Allies
                            .Where(a => a.isAlive)
                            .ToList(),

                    CombatTargetType.SELF_AND_PARTNER =>
                        Allies
                            .Where(a => a.isAlive)
                            .ToList(),

                    _ =>
                        new List<CombatantRuntime>
                        {
                            RandomAliveAlly()
                        }
                };

            hud.ShowTargetArrows(
                targets
                    .Where(t => t != null)
                    .Select(t => t.anchor)
            );

            foreach (var target in targets)
            {
                if (target == null)
                    continue;

                if (move.moveType == EnemyMoveType.ATTACK)
                {
                    int dmg =
                        Mathf.RoundToInt(
                            enemy.damage *
                            (
                                move.attackDamageMultiplier <= 0
                                    ? 1f
                                    : move.attackDamageMultiplier
                            )
                        );

                    int dealt =
                        target.ApplyDamage(dmg);

                    hud.ShowDescription(
                        string.IsNullOrEmpty(move.flavourText)
                            ? $"{enemy.displayName} attacks {target.displayName} for {dealt}!"
                            : move.flavourText
                    );
                }
                else if (move.ability != null)
                {
                    // New feature:
                    // Enemy abilities also use the multi-entry effect system.
                    foreach (var entry in move.ability.effects)
                    {
                        ApplyEffect(
                            enemy,
                            target,
                            entry
                        );
                    }

                    hud.ShowDescription(
                        string.IsNullOrEmpty(move.flavourText)
                            ? move.ability.description
                            : move.flavourText
                    );
                }
            }
        }

        CombatantRuntime RandomAliveAlly()
        {
            var alive =
                Allies
                    .Where(a => a.isAlive)
                    .ToList();

            return alive.Count == 0
                ? null
                : alive[Random.Range(0, alive.Count)];
        }

        // end condition
        void CheckDeaths()
        {
            foreach (var enemy in enemies)
            {
                hud.UpdateEnemyHealthBar(enemy);
            }

            if (enemies.All(e => !e.isAlive))
            {
                state = CombatState.BATTLE_WON;

                EndBattle(won: true);
                return;
            }

            bool playerDown = !player.isAlive;
            bool partnerDown =
                partner == null ||
                !partner.isAlive;

            if (playerDown && partnerDown)
            {
                state = CombatState.BATTLE_LOST;

                EndBattle(won: false);
            }
        }

        void EndBattle(bool won)
        {
            menuUI.HideAll();
            hud.ClearTargetArrows();

            hud.ShowDescription(
                state == CombatState.FLED
                    ? "Got away safely!"
                    : won
                        ? "Victory!"
                        : "You were defeated..."
            );

            input.InputEnabled = false;
            combatAssigner.WipeDataAssignment();
            // TODO:
            // Hook this into SceneSwitchController to return to the overworld,
            // show a reward screen, or trigger the game-over flow.
        }
    }
}