using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public interface ITrigger {
    Task<bool> Evaluate(FactMemory memory);
}

public abstract class DriveTrigger : ITrigger {
    public Drive Drive { get; }
    public DriveTrigger(Drive drive) {
        Drive = drive;
    }
    public abstract Task<bool> Evaluate(FactMemory memory);
}

public abstract class GoalTrigger : ITrigger {
    public Goal Goal { get; }
    public GoalTrigger(Goal goal) {
        Goal = goal;
    }
    public abstract Task<bool> Evaluate(FactMemory memory);
}

/// <summary>
/// a trigger that evaluates to true when the goal's satisfaction reaches a certain threshold
/// </summary>
public class GoalTriggerRemoveCompletedGoal : GoalTrigger {
    public float Threshold { get; }
    public GoalTriggerRemoveCompletedGoal(Goal goal, float threshold = 1.0f) : base(goal) {
        Threshold = threshold;
    }

    public override Task<bool> Evaluate(FactMemory memory) {
        return Task.FromResult(Goal.CurrentSatisfaction >= Threshold);
    }
}