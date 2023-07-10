using System.Threading.Tasks;
using Tacit.Framework.DGU;
using Tacit.Primer;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class HealGoal : Goal {
    public HealGoal(Drive drive) : base(drive) {
        Conditions.Add(
            new FuncPartialCondition(new FactChange(Drive.Agent.Id, FactAttributes.PERSON_HEALTH, FactChangeType.Increase), ScoreHealthIsHigh)
        );
        RemovalTriggers.Add(new GoalTriggerRemoveCompletedGoal(this));
    }
    public override long Weight => 10;
    public override async Task<float> Evaluate(FactMemory memory) {
        return await ScoreHealthIsHigh(memory);
    }

    private Task<float> ScoreHealthIsHigh(FactMemory memory) {
        var healthFact = memory.ExpectFact<float>(Drive.Agent.Id, FactAttributes.PERSON_HEALTH);
        var ret = Mathf.Clamp01(healthFact.Value / 0.9f);
        return Task.FromResult(ret);
    }
}