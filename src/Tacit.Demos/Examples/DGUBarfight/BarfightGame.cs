using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Minlog;
using Tacit.Demos.Examples.DGUBarfight.AI;
using Tacit.Demos.Util;
using Tacit.Framework.DGU;
using Tacit.Layers.Game;
using Tacit.Primer;

namespace Tacit.Demos.Examples.DGUBarfight;

public class BarfightGame : SimpleGame {
    private readonly ILogger _log;
    private readonly Random _rng = new();

    public LameECS ECS { get; }

    public BarfightGame(Logger log) {
        _log = log.For<BarfightGame>();
        ECS = new LameECS();
    }

    public void AddPerson(DrunkPersonAgent personAgent) {
        var entity = ECS.CreateEntity(personAgent.Name ?? "person");
        entity.AddComponent(personAgent);
        entity.AddComponent(new DrunkPersonStats(Constants.Values.HEALTH_MAX, 0));
    }

    public override async Task<Status> Update() {
        await base.Update();
        _log.Info($"Step {Steps}");

        // update action list for everyone
        foreach (var personEntity in ECS.GetEntitiesWithComponent<DrunkPersonAgent>()) {
            var personMind = personEntity.GetComponent<DrunkPersonAgent>();
            personMind.ConsumableActions.Clear();
            personMind.SuppliedActions.Clear();
            // drink alcohol self -> self
            personMind.ConsumableActions.Add(new DrinkAlcoholAction(personMind, personMind, Constants.Values.BEER_ABV));
            // punch someone self -> others
            foreach (var otherEntity in ECS.GetEntitiesWithComponent<DrunkPersonAgent>()) {
                var otherMind = otherEntity.GetComponent<DrunkPersonAgent>();
                if (otherEntity == personEntity) {
                    // we supply a getting punched action
                    personMind.SuppliedActions.Add(new ThrowPunchAction(personMind, otherMind));
                }
                else {
                    // and we can consume punch action on others
                    personMind.ConsumableActions.Add(new ThrowPunchAction(otherMind, personMind));
                }
            }
        }

        // update mind, get plan, and execute plan
        _log.Info($"Planning:");
        foreach (var personEntity in ECS.GetEntitiesWithComponent<DrunkPersonAgent>()) {
            _log.Info($"  Person entity {personEntity.Name}");
            // update mind
            var personMind = personEntity.GetComponent<DrunkPersonAgent>();
            await personMind.Update(Steps);
            // run planner
            var planCtx = new DGUPlanner.PlanInvocationContext(Steps);
            var personPlan = await personMind.Planner!.Plan(planCtx);
            _log.Info($"    Plan: {personPlan}");

            // execute plan
            _log.Info($"    Executing plan:");
            ExecutePlanActions(personPlan.Actions);
        }

        // show the game state (everyone's stats)
        _log.Info($"  Game state:");
        foreach (var personEntity in ECS.GetEntitiesWithComponent<DrunkPersonAgent>()) {
            var personMind = personEntity.GetComponent<DrunkPersonAgent>()!;
            var personStats = personEntity.GetComponent<DrunkPersonStats>()!;
            _log.Info($"    Person entity {personEntity.Name}:");
            _log.Info($"      Stats: {personStats}");

            // check end conditions/death conditions
            if (personStats.Health <= 0) {
                _log.Warn($"      Person entity {personEntity.Name} died!");
                ECS.DestroyEntity(personEntity);
            }
        }

        return Status.Continue;
    }

    private void ExecutePlanActions(List<VirtualAction> actions) {
        foreach (var action in actions) {
            _log.Info($"      Executing action: {action}");
            // switch on action type
            switch (action) {
                case DrinkAlcoholAction drinkAlcoholAction:
                    // drink alcohol
                    var drinkerEntity = ((DrunkPersonAgent)drinkAlcoholAction.Supplier!).Entity!;
                    var drinkerStats = drinkerEntity.GetComponent<DrunkPersonStats>()!;
                    var drinkBac = DrinkCalculator.CalculateDrinkBAC(Constants.Values.TYPICAL_GLASS_VOLUME,
                        drinkAlcoholAction.AlcoholStrength);
                    drinkerStats.Drunkenness += drinkBac;
                    drinkerStats.Health = Mathf.Clamp(drinkerStats.Health + Constants.Values.HEAL_FROM_DRINKING_GLASS,
                        0, Constants.Values.HEALTH_MAX);
                    break;
                case ThrowPunchAction throwPunchAction:
                    // hurt the target
                    var punchedEntity = ((DrunkPersonAgent)throwPunchAction.Supplier!).Entity!;
                    var punchedStats = punchedEntity.GetComponent<DrunkPersonStats>();
                    var puncherEntity = ((DrunkPersonAgent)throwPunchAction.Consumer!).Entity!;
                    var puncherStats = puncherEntity.GetComponent<DrunkPersonStats>();
                    // // for now use fixed punch damage
                    // var punchDamage = Constants.Values.BASE_PUNCH_DAMAGE;
                    // punchedStats.Health -= punchDamage;
                    var punchIsHit = _rng.NextDouble() < Constants.Values.PUNCH_HIT_PROBABILITY;
                    if (punchIsHit) {
                        var punchDamage = Constants.Values.BASE_PUNCH_DAMAGE;
                        punchedStats.Health -= punchDamage;
                        _log.Info($"        Punch hit! {punchDamage} damage");
                    }
                    else {
                        var whiffSelfDamage = Constants.Values.PUNCH_WHIFF_DAMAGE;
                        puncherStats.Health -= whiffSelfDamage;
                        _log.Info($"        Punch whiffed! {whiffSelfDamage} self damage");
                    }

                    break;
                default:
                    throw new Exception($"Unknown action type: {action.GetType()}");
            }
        }
    }
}