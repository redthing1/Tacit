using System.Collections.Generic;

namespace Tacit.Framework.DGU;

public class DGUPlanner {
    private readonly FactMemory _factMemory;
    private readonly List<Drive> _drives;
    private readonly List<Goal> _goals;
    
    public DGUPlanner(FactMemory factMemory, List<Drive> drives, List<Goal> goals) {
        _factMemory = factMemory;
        _drives = drives;
        _goals = goals;
    }
}