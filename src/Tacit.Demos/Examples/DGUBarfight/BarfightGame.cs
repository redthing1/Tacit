using System.Collections.Generic;
using System.Threading.Tasks;
using Minlog;
using Tacit.Demos.Examples.DGUBarfight.AI;
using Tacit.Demos.Util;
using Tacit.Framework.DGU;
using Tacit.Layers.Game;

namespace Tacit.Demos.Examples.DGUBarfight;

public record DrunkPersonStats(float Health, float Drunkenness);

public class BarfightGame : SimpleGame {
    private readonly ILogger _log;

    public LameECS ECS { get; }

    public BarfightGame(Logger log) {
        _log = log.For<BarfightGame>();
        ECS = new LameECS();
    }

    public void AddPerson(DrunkPersonMind personMind) {
        var entity = ECS.CreateEntity(personMind.Name ?? "person");
        entity.AddComponent(personMind);
        entity.AddComponent(new DrunkPersonStats(100, 0));
    }

    public override async Task<Status> Update() {
        await base.Update();
        _log.Info($"Step {Steps}");

        // update action list for everyone
        foreach (var personEntity in ECS.GetEntitiesWithComponent<DrunkPersonMind>()) {
            var personMind = personEntity.GetComponent<DrunkPersonMind>();
            personMind.ConsumableActions.Clear();
            personMind.SuppliedActions.Clear();
            // drink alcohol self -> self
            personMind.ConsumableActions.Add(new DrinkAlcoholAction(personMind, personMind));
            // punch someone self -> others
            foreach (var otherEntity in ECS.GetEntitiesWithComponent<DrunkPersonMind>()) {
                var otherMind = otherEntity.GetComponent<DrunkPersonMind>();
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

        foreach (var personEntity in ECS.GetEntitiesWithComponent<DrunkPersonMind>()) {
            var personMind = personEntity.GetComponent<DrunkPersonMind>();
            await personMind.Update(Steps);
            var planCtx = new DGUPlanner.PlanInvocationContext(Steps);
            var personPlan = await personMind.Planner!.Plan(planCtx);
            _log.Info($"  Person {personMind.Id} plan: {personPlan}");
        }

        return Status.Continue;
    }
}