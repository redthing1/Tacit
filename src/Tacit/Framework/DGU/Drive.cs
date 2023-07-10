using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public abstract class GoalGenerator {
    public record TriggeredGoal(ITrigger Trigger, Func<Goal> CreateGoal);

    public Drive Drive { get; }
    public List<TriggeredGoal> TriggeredGoals { get; } = new();

    public GoalGenerator(Drive drive) {
        Drive = drive;
    }

    public async Task<List<Goal>> GenerateGoals() {
        var createdGoals = new List<Goal>();
        // update all triggered goals
        foreach (var triggeredGoal in TriggeredGoals) {
            // check if the trigger is true
            if (await triggeredGoal.Trigger.Evaluate()) {
                // if so, create a new goal
                var newGoal = triggeredGoal.CreateGoal();
                createdGoals.Add(newGoal);
            }
        }

        return createdGoals;
    }
}

public abstract class Drive : IDGUDoctorable {
    public abstract string Name { get; }
    public abstract long Weight { get; }
    public GoalGenerator GoalGenerator { get; init; } = null!;
    public List<DriveTrigger> RemovalTriggers { get; } = new();

    public List<Goal> CurrentGoals { get; } = new();
    public float CurrentSatisfaction { get; protected set; }
    public DGUDoctor? Doctor { get; set; }

    public async Task Update(long time, FactMemory memory) {
        // evaluate current satisfaction
        CurrentSatisfaction = await Evaluate(memory);

        // check generator to see if more goals should be created
        var createdGoals = await GoalGenerator.GenerateGoals();
        
        // evaluate the newly created goals, then add them to the list of current goals
        foreach (var goal in createdGoals) {
            await goal.Update(time, memory);
            CurrentGoals.Add(goal);
        }
    }

    public abstract Task<float> Evaluate(FactMemory memory);
}