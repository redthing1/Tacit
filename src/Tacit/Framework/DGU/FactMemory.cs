using System.Collections.Generic;

namespace Tacit.Framework.DGU;

public class FactMemory {
    private List<IFact> _facts = new();
    private Dictionary<long, List<IFact>> _factsAtTime = new();

    public FactMemory() {
        Initialize();
    }
    
    public void Initialize() {
        // reset everything
        _facts.Clear();
        _factsAtTime.Clear();
    }
}