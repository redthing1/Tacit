using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class DrunkPerson : DGUAgent {
    public DrunkPerson(string id, AgentEnvironment environment) : base(id, environment) {
        Drives.Add(new StayAlive(this));
    }
}