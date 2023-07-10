using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public interface ITrigger {
    Task<bool> Evaluate();
}

public abstract class DriveTrigger : ITrigger {
    public Drive Drive { get; }
    public DriveTrigger(Drive drive) {
        Drive = drive;
    }
    public abstract Task<bool> Evaluate();
}

public abstract class GoalTrigger : ITrigger {
    public Goal Goal { get; }
    public GoalTrigger(Goal goal) {
        Goal = goal;
    }
    public abstract Task<bool> Evaluate();
}

public abstract class FactBasedTrigger : ITrigger {
    public FactMemory FactMemory { get; }
    public FactBasedTrigger(FactMemory factMemory) {
        FactMemory = factMemory;
    }

    public abstract Task<bool> Evaluate();
}

public abstract class GoalGeneratorTrigger : FactBasedTrigger {
    public Drive Drive { get; }
    protected GoalGeneratorTrigger(Drive drive, FactMemory factMemory) : base(factMemory) {
        Drive = drive;
    }
    public bool GoalExists(Func<Goal, bool> predicate) {
        return Drive.CurrentGoals.Any(predicate);
    }
}

public class GoalTriggerRemoveCompletedGoal : GoalTrigger {
    public GoalTriggerRemoveCompletedGoal(Goal goal) : base(goal) {}

    public override Task<bool> Evaluate() {
        return Task.FromResult(Goal.CurrentSatisfaction >= 1.0f);
    }
}