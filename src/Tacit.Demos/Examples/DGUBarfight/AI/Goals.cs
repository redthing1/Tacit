using System.Threading.Tasks;
using Tacit.Framework.DGU;
using Tacit.Primer;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class HealGoal : Goal {
    public HealGoal(Drive drive) : base(drive) {
        Conditions.Add(
            new FuncPartialCondition(new FactChange(Drive.Agent.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Increase), ScoreHealthIsHigh)
        );
        RemovalTriggers.Add(new GoalTriggerRemoveCompletedGoal(this));
    }
    public override long Weight => 10;
    public override async Task<float> Evaluate(FactMemory memory) {
        var score = await ScoreHealthIsHigh(memory);
        Drive.Agent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"{GetType().Name}::Evaluate: {score}");
        return score;
    }

    private Task<float> ScoreHealthIsHigh(FactMemory memory) {
        var healthFact = memory.ExpectFact<float>(Drive.Agent.Id, Constants.Facts.PERSON_HEALTH);
        var ret = Mathf.Clamp01(healthFact.Value / 0.9f);
        return Task.FromResult(ret);
    }
}

public class SoberUpGoal : Goal {
    public SoberUpGoal(Drive drive) : base(drive) {
        Conditions.Add(
            new FuncPartialCondition(new FactChange(Drive.Agent.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Decrease), ScoreDrunkennessIsLow)
        );
        RemovalTriggers.Add(new GoalTriggerRemoveCompletedGoal(this));
    }
    public override long Weight => 10;
    public override async Task<float> Evaluate(FactMemory memory) {
        var score = await ScoreDrunkennessIsLow(memory);
        Drive.Agent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"{GetType().Name}::Evaluate: {score}");
        return score;
    }

    private Task<float> ScoreDrunkennessIsLow(FactMemory memory) {
        var drunkennessFact = memory.ExpectFact<float>(Drive.Agent.Id, Constants.Facts.PERSON_DRUNKENNESS);
        var ret = Mathf.Clamp01(1 - drunkennessFact.Value / 0.9f);
        return Task.FromResult(ret);
    }
}