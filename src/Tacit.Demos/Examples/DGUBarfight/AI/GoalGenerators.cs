using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

class HealingGoalGenerator : GoalGenerator {
    public HealingGoalGenerator(Drive drive) : base(drive) {
    }

    public override Task<Goal[]> GenerateGoals(FactMemory memory) {
        // check if goal already exists
        if (GoalExists(x => x is HealMyselfGoal)) return Task.FromResult(Array.Empty<Goal>());
        // check if health is low
        var healthFact = memory.ExpectFact<float>(Drive.Agent.Id, Constants.Facts.PERSON_HEALTH);
        var healthIsLow = healthFact.Value < 0.5f;
        if (healthIsLow) {
            // create a goal to heal myself
            return Task.FromResult(new Goal[] {
                new HealMyselfGoal(Drive)
            });
        }

        return Task.FromResult(Array.Empty<Goal>());
    }
}

class SoberingGoalGenerator : GoalGenerator {
    public SoberingGoalGenerator(Drive drive) : base(drive) {
    }

    public override Task<Goal[]> GenerateGoals(FactMemory memory) {
        // check if goal already exists
        if (GoalExists(x => x is SoberUpGoal)) return Task.FromResult(Array.Empty<Goal>());
        // check if drunkenness is high
        var drunkennessFact = memory.ExpectFact<float>(Drive.Agent.Id, Constants.Facts.PERSON_DRUNKENNESS);
        var drunkennessIsHigh = drunkennessFact.Value > 0.5f;
        
        if (drunkennessIsHigh) {
            // create a goal to sober up
            return Task.FromResult(new Goal[] {
                new SoberUpGoal(Drive)
            });
        }
        
        return Task.FromResult(Array.Empty<Goal>());
    }
}

class BeatUpOthersGoalGenerator : GoalGenerator {
    public BeatUpOthersGoalGenerator(Drive drive) : base(drive) {
    }

    public override Task<Goal[]> GenerateGoals(FactMemory memory) {
        // find other people nearby and beat them up
        var allPeople = memory.ExpectFact<ISmartObject[]>(Drive.Agent.Id, Constants.Facts.ALL_PERSONS);
        
        var goals = new List<Goal>();
        foreach (var person in allPeople.Value) {
            if (person.Id == Drive.Agent.Id) continue; // skip self
            // see if we already have a goal to beat up this person
            if (GoalExists(x => x is BeatUpGoal beatUpGoal && beatUpGoal.Target == person)) continue;
        }
        
        return Task.FromResult(goals.ToArray());
    }
}