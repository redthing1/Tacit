using System.Linq;
using System.Threading.Tasks;
using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class MyStatsSensor : Sensor {
    public MyStatsSensor(DGUAgent agent) : base(agent) {
    }

    private new DrunkPersonMind Agent => (DrunkPersonMind)base.Agent;
    private BarfightEnvironment Environment => (BarfightEnvironment)Agent.Environment;

    public override Task Update(long time, FactMemory memory) {
        base.Update(time, memory);

        // var myEntity = Environment.Game.ECS.GetEntitiesWithComponent<DrunkPersonMind>()
        //     .Single(x => x.GetComponent<DrunkPersonMind>() == Agent);
        // var myStats = myEntity.GetComponent<DrunkPersonStats>();
        //
        // // create a fact for the agent's health
        // memory.AddFact(new Fact<float>(Agent, Constants.Facts.PERSON_HEALTH, myStats.Health, time));
        //
        // // create a fact for the agent's drunkenness
        // memory.AddFact(new Fact<float>(Agent, Constants.Facts.PERSON_DRUNKENNESS, myStats.Drunkenness, time));
        var allPeopleEntities = Environment.Game.ECS.GetEntitiesWithComponent<DrunkPersonMind>();
        
        foreach (var personEntity in allPeopleEntities) {
            var personMind = personEntity.GetComponent<DrunkPersonMind>();
            var personStats = personEntity.GetComponent<DrunkPersonStats>();
            
            // create a fact for the agent's health
            memory.AddFact(new Fact<float>(personMind, Constants.Facts.PERSON_HEALTH, personStats.Health, time));
            
            // create a fact for the agent's drunkenness
            memory.AddFact(new Fact<float>(personMind, Constants.Facts.PERSON_DRUNKENNESS, personStats.Drunkenness, time));
        }

        return Task.CompletedTask;
    }
}

public class EnvironmentObjectsSensor : Sensor {
    public EnvironmentObjectsSensor(DGUAgent agent) : base(agent) {
    }

    private BarfightEnvironment Environment => (BarfightEnvironment)Agent.Environment;

    public override Task Update(long time, FactMemory memory) {
        base.Update(time, memory);

        // create a fact for all objects in the environment
        var allPeopleEntities = Environment.Game.ECS.GetEntitiesWithComponent<DrunkPersonMind>();
        var allPeopleMinds = allPeopleEntities
            .Select(x => x.GetComponent<DrunkPersonMind>())
            .Cast<ISmartObject>()
            .ToArray();
        memory.AddFact(new Fact<ISmartObject[]>(Agent, Constants.Facts.ALL_PERSONS, allPeopleMinds, time));

        return Task.CompletedTask;
    }
}