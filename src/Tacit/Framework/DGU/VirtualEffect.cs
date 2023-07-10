namespace Tacit.Framework.DGU; 

public abstract class VirtualEffect {
    public FactChange Change { get; init; } = null!;
    public abstract IFact Simulate(IFact sourceFact);
}