using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Drives the whole battle: player/partner take one action per round (see the note on
    /// "Switch to Partner" below), then every alive enemy acts in order, repeat until someone wins.
    ///
    /// DESIGN ASSUMPTION (flag this with your team): only ONE ally acts per round, like a
    /// party-swap system. Picking "Switch to Partner" from the Actions menu hands the current
    /// round's single action to the partner instead of ending the round - it does not give both
    /// of them a turn in the same round. This was the cleanest reading of "Actions contains
    /// Switch to Partner" from the design doc; change BeginAllyRound()/OnActionsSubSelected()
    /// if you actually want both to act every round.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] PlayerLoadoutSO playerLoadout;
        [SerializeField] PartnerLoadoutSO partnerLoadout; // leave empty if partner isn't unlocked yet
        [SerializeField] EnemyDataSO[] enemyEncounter;

        [Header("Anchors")]
        [SerializeField] Transform playerAnchor;
        [SerializeField] Transform partnerAnchor;
        [SerializeField] Transform[] enemyAnchors;

        [Header("Linking Stuff")]
        [SerializeField] CombatInputReader input;
        [SerializeField] CombatHUD hud;
        [SerializeField] CombatMenuUI menuUI;
        [SerializeField] TargetSelector targetSelector;
        [SerializeField] FleeMinigameController flee;
        [SerializeField] AttackMasteryController mastery;

        CombatState state;
        CombatantRuntime player;
        CombatantRuntime partner; // null if not brought into this battle
        List<CombatantRuntime> enemies = new();

        CombatantRuntime activeAlly; // whichever of player/partner is acting this round

        List<CombatantRuntime> Allies => partner != null
            ? new List<CombatantRuntime> { player, partner }
            : new List<CombatantRuntime> { player };

        void Start()
        {
            SetupBattle();
        }

        void SetupBattle()
        {
            player = CombatantRuntime.FromPlayer(playerLoadout, playerAnchor);
            partner = partnerLoadout != null ? CombatantRuntime.FromPartner(partnerLoadout, partnerAnchor) : null;

            enemies.Clear();
            for (int i = 0; i < enemyEncounter.Length; i++)
            {
                Transform anchor = (enemyAnchors != null && i < enemyAnchors.Length) ? enemyAnchors[i] : null;
                enemies.Add(CombatantRuntime.FromEnemy(enemyEncounter[i], anchor));
            }

            targetSelector.Init(input, hud);
            menuUI.Init(input);
            flee.Init(input);
            mastery.Init(input);

            BeginAllyRound();
        }

        void BeginAllyRound()
        {
            player.isDefending = false;
            if (partner != null) partner.isDefending = false;

            activeAlly = player;
            ApplyStartOfTurnRegen(player);

            hud.UpdateStats(player.currentHP, player.maxHP, player.currentCS, player.maxCS);
            state = CombatState.MAIN_MENU;
            ShowMenuFor(player, isPlayer: true);
        }

        void ApplyStartOfTurnRegen(CombatantRuntime actor)
        {
            if (actor.playerSource == null) return; // regen accessories are a player-only concept for now
            actor.Heal(actor.playerSource.TotalHPRegen());
            actor.GainCurse(actor.playerSource.TotalCSRegen());
        }

        void ShowMenuFor(CombatantRuntime actor, bool isPlayer)
        {
            state = CombatState.MAIN_MENU;
            var options = isPlayer
                ? new List<CombatMenuOption> { CombatMenuOption.COMBAT, CombatMenuOption.ACTIONS, CombatMenuOption.ITEMS, CombatMenuOption.FLEE }
                : new List<CombatMenuOption> { CombatMenuOption.COMBAT, CombatMenuOption.ACTIONS }; // partner list

            menuUI.ShowIconRow(options, actor.anchor, idx => OnMainMenuSelected(actor, isPlayer, (CombatMenuOption)idx), () => { /* nothing to cancel back to at the root */ });
        }

        void OnMainMenuSelected(CombatantRuntime actor, bool isPlayer, CombatMenuOption option)
        {
            switch (option)
            {
                case CombatMenuOption.COMBAT:
                    state = CombatState.SUB_MENU;
                    menuUI.ShowSubmenu(new List<string> { "Attack", "Defend" }, actor.anchor,
                        idx => OnCombatSubSelected(actor, idx),
                        () => ShowMenuFor(actor, isPlayer));
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
            bool showSwitch = isPlayer && partner != null && partner.isAlive;
            List<AbilitySO> abilities = actor.playerSource != null ? actor.playerSource.knownAbilities : actor.partnerSource.knownAbilities;

            List<string> labels = new();
            if (showSwitch) labels.Add("Switch to Partner");
            foreach (var a in abilities) labels.Add($"{a.abilityName} ({a.curseCost} Cs)");

            state = CombatState.SUB_MENU;
            menuUI.ShowSubmenu(labels, actor.anchor, idx =>
            {
                if (showSwitch && idx == 0)
                {
                    activeAlly = partner;
                    hud.ShowDescription($"{player.displayName} swaps in {partner.displayName}!");
                    ShowMenuFor(partner, isPlayer: false);
                    return;
                }

                int abilityIndex = showSwitch ? idx - 1 : idx;
                AbilitySO ability = abilities[abilityIndex];
                OnAbilitySelected(actor, ability);
            }, () => ShowMenuFor(actor, isPlayer));
        }

        void OnAbilitySelected(CombatantRuntime actor, AbilitySO ability)
        {
            if (actor.currentCS < ability.curseCost)
            {
                hud.ShowDescription("Not enough curse energy!");
                return; // stays on submenu, let them pick something else
            }

            hud.ShowDescription(ability.description);
            state = CombatState.TARGET_SELECT;
            targetSelector.BeginSelection(ability.targetType, actor, Allies, enemies,
                targets => ResolveAbility(actor, ability, targets),
                () => ShowActionsMenu(actor, actor == player));
        }

        void ShowItemsMenu(CombatantRuntime actor, bool isPlayer)
        {
            // items are player only
            if (actor.playerSource == null) return;

            var inventory = actor.playerSource.inventory;
            List<string> labels = inventory.Select(stack => $"{stack.item.itemName} x{stack.count}").ToList();

            state = CombatState.SUB_MENU;
            menuUI.ShowSubmenu(labels, actor.anchor, idx =>
            {
                var stack = inventory[idx];
                if (stack.count <= 0) return;
                OnItemSelected(actor, idx, stack.item);
            }, () => ShowMenuFor(actor, isPlayer));
        }

        void OnItemSelected(CombatantRuntime actor, int inventoryIndex, ItemSO item)
        {
            hud.ShowDescription(item.description);
            state = CombatState.TARGET_SELECT;
            targetSelector.BeginSelection(item.targetType, actor, Allies, enemies,
                targets =>
                {
                    ResolveItem(actor, item, targets);
                    if (item.consumeOnUse)
                    {
                        var stack = actor.playerSource.inventory[inventoryIndex];
                        stack.count = Mathf.Max(stack.count - 1, 0);
                        actor.playerSource.inventory[inventoryIndex] = stack;
                    }
                },
                () => ShowItemsMenu(actor, true));
        }

        void OnCombatSubSelected(CombatantRuntime actor, int idx)
        {
            var subOption = (CombatSubOption)idx;
            if (subOption == CombatSubOption.ATTACK)
            {
                state = CombatState.TARGET_SELECT;
                targetSelector.BeginSelection(CombatTargetType.SINGLE_ENEMY, actor, Allies, enemies,
                    targets => ResolveAttack(actor, targets[0]),
                    () => OnMainMenuSelected(actor, actor == player, CombatMenuOption.COMBAT));
            }
            else // DEFEND
            {
                ResolveDefend(actor);
            }
        }

        void ResolveAttack(CombatantRuntime actor, CombatantRuntime target)
        {
            hud.ClearTargetArrows();

            // attack mastery currently skips for the partner/enemies for now (AttackMasteryController todo change later).
            if (actor == player)
            {
                float width = 0.25f * playerLoadout.TotalMasteryWidthMultiplier(); // TODO: tune base window width
                mastery.BeginAttempt(width, bonusHit => FinishAttack(actor, target, bonusHit ? 1 : 0));
            }
            else
            {
                FinishAttack(actor, target, 0);
            }
        }

        void FinishAttack(CombatantRuntime actor, CombatantRuntime target, int masteryBonus)
        {
            int dealt = target.ApplyDamage(actor.damage + masteryBonus);
            hud.ShowDescription($"{actor.displayName} hits {target.displayName} for {dealt}!");
            CheckDeaths();
            EndAllyAction();
        }

        void ResolveDefend(CombatantRuntime actor)
        {
            actor.isDefending = true;
            actor.GainCurse(5); // TODO: tune curse gained from defending
            hud.ShowDescription($"{actor.displayName} braces for the next attack.");
            EndAllyAction();
        }

        void ResolveAbility(CombatantRuntime actor, AbilitySO ability, List<CombatantRuntime> targets)
        {
            hud.ClearTargetArrows();
            actor.SpendCurse(ability.curseCost);

            foreach (var target in targets)
                ApplyEffect(actor, target, ability.effect, ability.power);

            hud.ShowDescription($"{actor.displayName} uses {ability.abilityName}!");
            CheckDeaths();
            EndAllyAction();
        }

        void ResolveItem(CombatantRuntime actor, ItemSO item, List<CombatantRuntime> targets)
        {
            hud.ClearTargetArrows();

            foreach (var target in targets)
                ApplyEffect(actor, target, item.effect, item.amount);

            hud.ShowDescription($"{actor.displayName} uses {item.itemName}!");
            CheckDeaths();
            EndAllyAction();
        }

        void ApplyEffect(CombatantRuntime source, CombatantRuntime target, EffectType effect, int power)
        {
            switch (effect)
            {
                case EffectType.DAMAGE: target.ApplyDamage(power); break;
                case EffectType.HEAL_HP: target.Heal(power); break;
                case EffectType.HEAL_CS: target.GainCurse(power); break;
                case EffectType.BUFF_DAMAGE: target.damage += power; break;      // TODO: decide if buffs are permanent or should expire after N turns
                case EffectType.BUFF_DEFENSE: target.defense += power; break;
                case EffectType.INVINCIBLE: target.isInvincible = true; break;
                case EffectType.CURE_STATUS: /* TODO: hook up once status effects exist */ break;
            }
        }

        void AttemptFlee()
        {
            int combined = enemies.Where(e => e.isAlive).Sum(e => e.damage + e.defense);
            flee.SetRequiredPressesFromEnemies(combined);

            state = CombatState.FLEE_MINIGAME;
            hud.ShowDescription("Mash Z and X to break free!");
            menuUI.HideAll();

            flee.BeginAttempt(success =>
            {
                if (success)
                {
                    state = CombatState.FLED;
                    hud.ShowDescription("Got away safely!");
                    EndBattle(won: false);
                }
                else
                {
                    hud.ShowDescription("Couldn't escape!");
                    EndAllyAction();
                }
            });
        }

        void EndAllyAction()
        {
            if (state == CombatState.BATTLE_WON || state == CombatState.BATTLE_LOST || state == CombatState.FLED)
                return;

            menuUI.HideAll();
            hud.ClearTargetArrows();
            BeginEnemyTurn();
        }

        void BeginEnemyTurn()
        {
            state = CombatState.ENEMY_TURN;

            /* TODO: this resolves every enemy's move back-to-back with no pause, once you have
            sprites/animations, drive this with a coroutine or DelayedCallbackInvoker so each
            hit has a beat before the next one starts. */
            foreach (var enemy in enemies.ToList())
            {
                if (!enemy.isAlive) continue;
                var move = EnemyFSMController.GetNextMove(enemy);
                ResolveEnemyMove(enemy, move);

                if (!player.isAlive && (partner == null || !partner.isAlive))
                    break; // party wiped mid-round, no point resolving further hits
            }

            CheckDeaths();

            if (state == CombatState.BATTLE_WON || state == CombatState.BATTLE_LOST)
                return;

            BeginAllyRound();
        }

        void ResolveEnemyMove(CombatantRuntime enemy, EnemyMove move)
        {
            List<CombatantRuntime> targets = move.targetType switch
            {
                CombatTargetType.SELF => new List<CombatantRuntime> { enemy },
                CombatTargetType.ALL_ENEMIES => Allies.Where(a => a.isAlive).ToList(),   // "enemies" from the attacking enemy pov is the player's side
                CombatTargetType.SELF_AND_PARTNER => Allies.Where(a => a.isAlive).ToList(),
                _ => new List<CombatantRuntime> { RandomAliveAlly() },
            };

            hud.ShowTargetArrows(targets.Where(t => t != null).Select(t => t.anchor));

            foreach (var target in targets)
            {
                if (target == null) continue;

                if (move.moveType == EnemyMoveType.ATTACK)
                {
                    int dmg = Mathf.RoundToInt(enemy.damage * (move.attackDamageMultiplier <= 0 ? 1f : move.attackDamageMultiplier));
                    int dealt = target.ApplyDamage(dmg);
                    hud.ShowDescription(string.IsNullOrEmpty(move.flavourText)
                        ? $"{enemy.displayName} attacks {target.displayName} for {dealt}!"
                        : move.flavourText);
                }
                else if (move.ability != null)
                {
                    ApplyEffect(enemy, target, move.ability.effect, move.ability.power);
                    hud.ShowDescription(string.IsNullOrEmpty(move.flavourText) ? move.ability.description : move.flavourText);
                }
            }
        }

        CombatantRuntime RandomAliveAlly()
        {
            var alive = Allies.Where(a => a.isAlive).ToList();
            return alive.Count == 0 ? null : alive[Random.Range(0, alive.Count)];
        }

        void CheckDeaths()
        {
            if (enemies.All(e => !e.isAlive))
            {
                state = CombatState.BATTLE_WON;
                EndBattle(won: true);
                return;
            }

            bool playerDown = !player.isAlive;
            bool partnerDown = partner == null || !partner.isAlive;
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
            hud.ShowDescription(state == CombatState.FLED ? "Got away safely!" : (won ? "Victory!" : "You were defeated..."));
            input.InputEnabled = false;

            // TODO: hook this into SceneSwitchController (see your project's SceneSwitchController.cs to return to the overworld / show a reward screen / trigger a game-over flow.
        }
    }
}
