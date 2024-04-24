using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

/// <summary>
/// contains all actions being supplied by smart objects in the environment
/// </summary>
public record AllEnvironmentActions(List<VirtualAction> Actions);

public abstract class VirtualAction {
    public virtual string Name => GetType().Name;
    // public abstract long Weight { get; }

    public List<IPartialCondition> Preconditions { get; private set; } = new();
    public List<VirtualEffect> Effects { get; private set; } = new();
    /// <summary>
    /// the object the action is performed on
    /// </summary>
    public ISmartObject? Supplier { get; init; }
    /// <summary>
    /// the agent that performs the action
    /// </summary>
    public ISmartObject? Consumer { get; init; }

    protected VirtualAction(ISmartObject? supplier, ISmartObject? consumer) {
        Supplier = supplier;
        Consumer = consumer;
    }

    // public abstract Task<List<IFact>> SimulateWorldEffects(in DGUPlanState state);

    public VirtualAction Fork() {
        // create a deep copy of this action
        var ret = (VirtualAction)MemberwiseClone();
        ret.Preconditions = new List<IPartialCondition>(Preconditions);
        ret.Effects = new List<VirtualEffect>(Effects);
        return ret;
    }

    public override string ToString() {
        return $"{Name}";
    }
}