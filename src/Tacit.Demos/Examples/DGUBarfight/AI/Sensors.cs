using System.Threading.Tasks;
using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class MyStatsSensor : Sensor {
    public MyStatsSensor(DGUAgent agent) : base(agent) {}

    new private DrunkPerson Agent => (DrunkPerson)base.Agent;
    private BarfightEnvironment Environment => (BarfightEnvironment)Agent.Environment;

    public override Task Update(long time, FactMemory memory) {
        base.Update(time, memory);

        var myStats = Environment.Game.PersonStatsMap[Agent];
        // create a fact for the agent's health
        memory.AddFact(new Fact<float>(Agent, FactAttributes.PERSON_HEALTH, myStats.Health, time));

        return Task.CompletedTask;
    }
}