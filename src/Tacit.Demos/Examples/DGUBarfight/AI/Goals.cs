using System.Threading.Tasks;
using Tacit.Framework.DGU;
using Tacit.Primer;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class HealMyselfGoal : Goal {
    public HealMyselfGoal(Drive drive) : base(drive) {
        // this goal can be satisfied by increasing the health fact
        Conditions.Add(
            new FuncPartialCondition(
                new FactChange(Drive.Agent.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Increase),
                ScoreHealthIsHigh)
        );
        RemovalTriggers.Add(new GoalTriggerRemoveCompletedGoal(this));
    }

    public override long Weight => 20;

    public override async Task<float> Evaluate(FactMemory memory) {
        var score = await ScoreHealthIsHigh(memory);
        // Drive.Agent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"{GetType().Name}::Evaluate: {score}");
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
        // this goal can be satisfied by decreasing the drunkenness fact
        Conditions.Add(
            new FuncPartialCondition(
                new FactChange(Drive.Agent.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Decrease),
                ScoreDrunkennessIsLow)
        );
        RemovalTriggers.Add(new GoalTriggerRemoveCompletedGoal(this));
    }

    public override long Weight => 20;

    public override async Task<float> Evaluate(FactMemory memory) {
        var score = await ScoreDrunkennessIsLow(memory);
        // Drive.Agent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"{GetType().Name}::Evaluate: {score}");
        return score;
    }

    private Task<float> ScoreDrunkennessIsLow(FactMemory memory) {
        var drunkennessFact = memory.ExpectFact<float>(Drive.Agent.Id, Constants.Facts.PERSON_DRUNKENNESS);
        var scaledDrunkenness = Mathf.Map01Clamp01(drunkennessFact.Value, Constants.Values.SOBER_ENOUGH,
            Constants.Values.DANGEROUS_DRUNKENNESS);
        var ret = 1f - scaledDrunkenness;
        return Task.FromResult(ret);
    }
}

class BeatUpGoal : Goal {
    public ISmartObject Target { get; }
    public override long Weight => 10;

    public BeatUpGoal(Drive drive, ISmartObject target) : base(drive) {
        Target = target;
        // this goal can be satisfied by decreasing the health fact of the target
        Conditions.Add(
            new FuncPartialCondition(
                new FactChange(Target.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Decrease),
                ScoreTargetHealthIsLow)
        );
        RemovalTriggers.Add(new GoalTriggerRemoveCompletedGoal(this));
    }

    private Task<float> ScoreTargetHealthIsLow(FactMemory memory) {
        // check the health of the target
        var healthFact = memory.ExpectFact<float>(Target.Id, Constants.Facts.PERSON_HEALTH);
        var healthPercent = Mathf.Clamp01(healthFact.Value / Constants.Values.HEALTH_MAX);
        var ret = 1f - healthPercent;
        return Task.FromResult(ret);
    }

    public override Task<float> Evaluate(FactMemory memory) {
        return ScoreTargetHealthIsLow(memory);
    }
}