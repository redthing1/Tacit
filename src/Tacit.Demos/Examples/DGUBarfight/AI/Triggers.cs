using System.Threading.Tasks;
using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class LowHealthTrigger : GoalGeneratorTrigger {
    public LowHealthTrigger(DGUAgent agent, Drive drive) : base(agent, drive) {}

    public override Task<bool> Evaluate(FactMemory memory) {
        // check if goal already exists
        if (GoalExists(x => x is HealGoal)) ;
        // check if health is low
        var healthFact = memory.ExpectFact<float>(Agent.Id, Constants.Facts.PERSON_HEALTH);
        var healthIsLow = healthFact.Value < 0.5f;
        Agent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"{GetType().Name}::Evaluate: {healthIsLow}");
        return Task.FromResult(healthIsLow);
    }
}

public class HighDrunkennessTrigger : GoalGeneratorTrigger {
    public HighDrunkennessTrigger(DGUAgent agent, Drive drive) : base(agent, drive) {}

    public override Task<bool> Evaluate(FactMemory memory) {
        // check if goal already exists
        if (GoalExists(x => x is SoberUpGoal)) ;
        // check if drunkenness is high
        var drunkennessFact = memory.ExpectFact<float>(Agent.Id, Constants.Facts.PERSON_DRUNKENNESS);
        var drunkennessIsHigh = drunkennessFact.Value > 0.5f;
        Agent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"{GetType().Name}::Evaluate: {drunkennessIsHigh}");
        return Task.FromResult(drunkennessIsHigh);
    }
}