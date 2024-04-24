using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public abstract class GoalGenerator {
    public Drive Drive { get; }

    public GoalGenerator(Drive drive) {
        Drive = drive;
    }

    protected bool GoalExists(Func<Goal, bool> predicate) {
        return Drive.CurrentGoals.Any(predicate);
    }

    // public async Task<List<Goal>> GenerateGoals(FactMemory memory) {
    //     var createdGoals = new List<Goal>();
    //     // update all triggered goals
    //     foreach (var triggeredGoal in TriggeredGoals) {
    //         // check if the trigger is true
    //         if (await triggeredGoal.Trigger.Evaluate(memory)) {
    //             // if so, create a new goal
    //             var newGoal = triggeredGoal.CreateGoal();
    //             createdGoals.Add(newGoal);
    //         }
    //     }
    //
    //     return createdGoals;
    // }
    public abstract Task<Goal[]> GenerateGoals(FactMemory memory);
}

public abstract class Drive {
    public DGUAgent Agent { get; }
    public virtual string Name => GetType().Name;
    public abstract long Weight { get; }
    public List<GoalGenerator> GoalGenerators { get; } = new();
    public List<DriveTrigger> RemovalTriggers { get; } = new();
    public List<Goal> CurrentGoals { get; } = new();
    public float CurrentSatisfaction { get; protected set; }

    public Drive(DGUAgent agent) {
        Agent = agent;
    }

    public virtual async Task Update(long time, FactMemory memory) {
        // evaluate current satisfaction
        CurrentSatisfaction = await Evaluate(memory);
        
        Agent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"drive({GetType().Name})::Update: CurrentSatisfaction: {CurrentSatisfaction}");

        // check generators to see if more goals should be created
        // var createdGoals = await GoalGenerator.GenerateGoals(memory);
        var newGoals = new List<Goal>();
        foreach (var generator in GoalGenerators) {
            var createdGoals = await generator.GenerateGoals(memory);
            if (createdGoals.Length <= 0) continue;
            Agent.Doctor?.Log(DGUDoctor.LogLevel.Debug,
                $"  {generator.GetType().Name} created goals: {createdGoals.Length}");
            newGoals.AddRange(createdGoals);
        }

        // evaluate the newly created goals, then add them to the list of current goals
        foreach (var goal in newGoals) {
            await goal.Update(time, memory);
            // add it to both the drive's goal list and the agent's goal list
            CurrentGoals.Add(goal);
            Agent.Goals.Add(goal);
        }
    }

    public virtual async Task<float> Evaluate(FactMemory memory) {
        if (CurrentGoals.Count == 0) return 1; // if there are no goals, we are satisfied
        float totalSatisfaction = 0;
        long totalWeight = 0;
        foreach (var goal in CurrentGoals) {
            totalSatisfaction += await goal.Evaluate(memory) * goal.Weight;
            totalWeight += goal.Weight;
        }

        if (totalWeight == 0) return 0;
        var score = totalSatisfaction / totalWeight;
        // Agent.Doctor?.Log(DGUDoctor.LogLevel.Debug, $"{GetType().Name}::Evaluate: {score}");
        return score;
    }
}