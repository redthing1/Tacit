using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public class DGUPlanner {
    private int _idCounter = 0;
    public DGUAgent RootAgent { get; }
    public DGUPlanner(DGUAgent rootAgent) {
        RootAgent = rootAgent;
    }
    
    private int GetNextId() => _idCounter++;

    public async Task<Plan> Plan() {
        // get seed plan states
        var planStates = await GeneratePlanStatesFromInitial();

        while (planStates.Count > 0) {
            
        }
        
        throw new NotImplementedException();
    }

    private Task<List<DGUPlanState>> GeneratePlanStatesFromInitial() {
        var states = new List<DGUPlanState>();
        var agent = RootAgent;
        
        // generate a root plan state
        var rootState = new DGUPlanState(GetNextId(), hardConditions: null,
    }
}