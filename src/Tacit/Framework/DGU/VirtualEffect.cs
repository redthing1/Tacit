using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public record WorldDiff(FactChange Change, IFact Fact, long depth);
    
public abstract class VirtualEffect {
    public FactChange Change { get; init; }
    
    public abstract Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff);
    
    protected VirtualEffect(FactChange change) {
        Change = change;
    }
}