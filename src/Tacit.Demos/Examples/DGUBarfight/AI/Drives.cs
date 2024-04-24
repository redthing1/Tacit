using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI; 


public class StayAliveDrive : Drive {
    public override long Weight { get; } = 1;

    public StayAliveDrive(DGUAgent agent) : base(agent) {
        // set up goal generators
        GoalGenerators.Add(new HealingGoalGenerator(this));
        GoalGenerators.Add(new SoberingGoalGenerator(this));
    }
}

public class BeatUpOthersDrive : Drive {
    public override long Weight { get; } = 2;

    public BeatUpOthersDrive(DGUAgent agent) : base(agent) {
        // set up goal generators
        GoalGenerators.Add(new BeatUpOthersGoalGenerator(this));
    }
}
