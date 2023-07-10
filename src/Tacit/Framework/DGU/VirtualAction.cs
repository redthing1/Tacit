using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public abstract class VirtualAction {
    public abstract string Name { get; }
    public abstract long Weight { get; }

    public List<PartialCondition> Preconditions { get; } = new();
    public List<VirtualEffect> Effects { get; } = new();
    public ISmartObject Supplier { get; init; } = null!;
    public ISmartObject Consumer { get; init; } = null!;

    public abstract Task SimulateWorldEffects();
}