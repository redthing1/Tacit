using Tacit.Framework.DGU;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class DrunkPersonMind : DGUAgent {
    public DrunkPersonMind(string id, AgentEnvironment environment) : base(id, environment) {
        Sensors.Add(new MyStatsSensor(this));
        Sensors.Add(new EnvironmentObjectsSensor(this));
            
        Drives.Add(new StayAlive(this));
        Drives.Add(new BeatUpOthers(this));

        ConsumableActions.Add(new DrinkAlcoholAction(supplier: this, consumer: this));
    }
}