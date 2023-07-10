using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public class GoalGenerator {
    public record TriggeredGoal(ITrigger Trigger, Func<Goal> CreateGoal);

    public Drive Drive { get; }
    public List<TriggeredGoal> TriggeredGoals { get; } = new();

    public GoalGenerator(Drive drive) {
        Drive = drive;
    }

    public async Task<List<Goal>> GenerateGoals(FactMemory memory) {
        var createdGoals = new List<Goal>();
        // update all triggered goals
        foreach (var triggeredGoal in TriggeredGoals) {
            // check if the trigger is true
            if (await triggeredGoal.Trigger.Evaluate(memory)) {
                // if so, create a new goal
                var newGoal = triggeredGoal.CreateGoal();
                createdGoals.Add(newGoal);
            }
        }

        return createdGoals;
    }
}

public abstract class Drive {
    public DGUAgent Agent { get; }
    public virtual string Name => GetType().Name;
    public abstract long Weight { get; }
    public GoalGenerator GoalGenerator { get; } = null!;
    public List<DriveTrigger> RemovalTriggers { get; } = new();
    public List<Goal> CurrentGoals { get; } = new();
    public float CurrentSatisfaction { get; protected set; }
    
    public Drive(DGUAgent agent) {
        Agent = agent;
        GoalGenerator = new(this);
    }

    public virtual async Task Update(long time, FactMemory memory) {
        // evaluate current satisfaction
        CurrentSatisfaction = await Evaluate(memory);

        // check generator to see if more goals should be created
        var createdGoals = await GoalGenerator.GenerateGoals(memory);

        // evaluate the newly created goals, then add them to the list of current goals
        foreach (var goal in createdGoals) {
            await goal.Update(time, memory);
            CurrentGoals.Add(goal);
        }
    }

    public virtual async Task<float> Evaluate(FactMemory memory) {
        float totalSatisfaction = 0;
        long totalWeight = 0;
        foreach (var goal in CurrentGoals) {
            totalSatisfaction += await goal.Evaluate(memory) * goal.Weight;
            totalWeight += goal.Weight;
        }
        if (totalWeight == 0) return 0;
        var score = totalSatisfaction / totalWeight;
        Agent.Doctor?.Log(DGUDoctor.LogLevel.Trace, $"{GetType().Name}::Evaluate: {score}");
        return score;
    }
}