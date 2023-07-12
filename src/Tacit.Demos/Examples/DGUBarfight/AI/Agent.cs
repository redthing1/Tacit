using Tacit.Demos.Util;
using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class DrunkPersonAgent : DGUAgent, IComponent {
    public DrunkPersonAgent(string name, AgentEnvironment environment) : base(id: name, environment) {
        Name = name;
        Sensors.Add(new MyStatsSensor(this));
        Sensors.Add(new EnvironmentObjectsSensor(this));

        Drives.Add(new StayAliveDrive(this));
        Drives.Add(new BeatUpOthersDrive(this));
    }

    public Entity? Entity { get; set; }
}

public class DrunkPersonStats : IComponent {
    public DrunkPersonStats(float health, float drunkenness) {
        Health = health;
        Drunkenness = drunkenness;
    }

    public float Health { get; set; }
    public float Drunkenness { get; set; }

    public override string ToString() {
        return $"(health={Health}, drunkenness={Drunkenness})";
    }

    public Entity? Entity { get; set; }
}