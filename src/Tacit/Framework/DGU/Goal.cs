using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public class GoalCondition {}

public abstract class Goal {
    public abstract string Name { get; }
    public abstract long Weight { get; }

    public Drive DriveGeneratedBy { get; init; } = null!;
    public List<GoalCondition> Conditions { get; } = new();
    public List<GoalTrigger> RemovalTriggers { get; } = new();

    public float CurrentSatisfaction { get; protected set; }    
    
    protected Goal(Drive driveGeneratedBy) {
        DriveGeneratedBy = driveGeneratedBy;
    }

    public async Task Update() {
        CurrentSatisfaction = await Evaluate();
    }

    protected abstract Task<float> Evaluate();
}