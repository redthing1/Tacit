using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public class DGUPlanState {
    public int Id { get; init; }

    /// <summary>
    /// must be satisfied as part of plan formulation
    /// </summary>
    public List<PartialCondition> HardConditions { get; } = new();

    /// <summary>
    /// desirable to satisfy, but not required
    /// </summary>
    public List<PartialCondition> SoftConditions { get; } = new();

    public DGUPlanState? Parent { get; set; }
    public VirtualAction ActionGeneratedBy { get; set; } = null!;

    public float Utility { get; set; }

    public DGUPlanState(int id, List<PartialCondition> hardConditions, List<PartialCondition> softConditions, DGUPlanState? parent, VirtualAction actionGeneratedBy) {
        Id = id;
        HardConditions = hardConditions;
        SoftConditions = softConditions;
        Parent = parent;
        ActionGeneratedBy = actionGeneratedBy;
    }

    public List<VirtualAction> CollectPredecessorActions() {
        // walk up the tree and collect all actions
        var actionPredecessors = new List<VirtualAction>();
        var current = this;
        while (current.Parent != null) {
            actionPredecessors.Add(current.ActionGeneratedBy);
            current = current.Parent;
        }
        // reverse the list so that the actions are in chronological order
        actionPredecessors.Reverse();

        return actionPredecessors;
    }

    public async Task Update() {
        Utility = await Evaluate();
    }

    protected async Task<float> Evaluate() {
        throw new System.NotImplementedException();
    }
}