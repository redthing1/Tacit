using System.Threading.Tasks;
using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class LowHealthTrigger : GoalGeneratorTrigger {
    public LowHealthTrigger(DGUAgent agent, Drive drive) : base(agent, drive) {}

    public override Task<bool> Evaluate(FactMemory memory) {
        // check if goal already exists
        if (GoalExists(x => x is HealGoal)) ;
        // check if health is low
        var healthFact = memory.ExpectFact<float>(Agent.Id, FactAttributes.PERSON_HEALTH);
        var healthIsLow = healthFact.Value < 0.5f;
        return Task.FromResult(healthIsLow);
    }
}