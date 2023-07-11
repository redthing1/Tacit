using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI; 


public class StayAlive : Drive {
    public override long Weight { get; } = 1;

    public StayAlive(DGUAgent agent) : base(agent) {
        // set up goal generators
        GoalGenerator.TriggeredGoals.Add(
            new GoalGenerator.TriggeredGoal(
                new LowHealthTrigger(agent, this),
                () => new HealGoal(this))
        );
        GoalGenerator.TriggeredGoals.Add(
            new GoalGenerator.TriggeredGoal(
                new HighDrunkennessTrigger(agent, this),
                () => new SoberUpGoal(this))
        );
    }
}
