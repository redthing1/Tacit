using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI; 


public class StayAlive : Drive {
    public override long Weight { get; } = 1;

    public StayAlive(DGUAgent agent) : base(agent) {
        // set up goal generators
        GoalGenerators.Add(new HealingGoalGenerator(this));
        GoalGenerators.Add(new SoberingGoalGenerator(this));
    }
}

public class BeatUpOthers : Drive {
    public override long Weight { get; } = 2;

    public BeatUpOthers(DGUAgent agent) : base(agent) {
        // set up goal generators
        GoalGenerators.Add(new BeatUpOthersGoalGenerator(this));
    }
}
