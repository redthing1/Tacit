using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class DrunkPersonMind : DGUAgent {
    public DrunkPersonMind(string id, AgentEnvironment environment) : base(id, environment) {
        Sensors.Add(new MyStatsSensor(this));
        Sensors.Add(new EnvironmentObjectsSensor(this));
            
        Drives.Add(new StayAliveDrive(this));
        Drives.Add(new BeatUpOthersDrive(this));
    }
}