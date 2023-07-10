using System.Collections.Generic;
using System.Threading.Tasks;
using Tacit.Utils;

namespace Tacit.Framework.DGU;

public class DGUPlanState : IForkable<DGUPlanState> {
    public int Id { get; init; }

    /// <summary>
    /// must be satisfied as part of plan formulation
    /// </summary>
    public List<IPartialCondition>? HardGoalConditions { get; private set; } = new();

    /// <summary>
    /// desirable to satisfy, but not required
    /// </summary>
    public List<IPartialCondition>? SoftGoalConditions { get; private set; } = new();

    public FactMemory HypotheticalFacts { get; private set; } = null!;

    public DGUPlanState? Parent { get; set; }
    public VirtualAction? ActionGeneratedBy { get; set; } = null!;
    public float Score { get; set; }

    public DGUPlanState(int id, List<IPartialCondition>? hardGoalConditions, List<IPartialCondition>? softGoalConditions, DGUPlanState? parent, FactMemory hypotheticalFacts, VirtualAction? actionGeneratedBy) {
        Id = id;
        HardGoalConditions = hardGoalConditions;
        SoftGoalConditions = softGoalConditions;
        Parent = parent;
        HypotheticalFacts = hypotheticalFacts;
        ActionGeneratedBy = actionGeneratedBy;
    }

    public DGUPlanState Fork() {
        var ret = (DGUPlanState)MemberwiseClone();
        // deep copy
        ret.HypotheticalFacts = HypotheticalFacts.Fork();
        ret.HardGoalConditions = new List<IPartialCondition>(HardGoalConditions);
        ret.SoftGoalConditions = new List<IPartialCondition>(SoftGoalConditions);
        return ret;
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

    public async Task<List<IPartialCondition>> GetUnsatisfiedPreconditions() {
        var conditions = new List<IPartialCondition>();
        conditions.AddRange(HardGoalConditions);
        conditions.AddRange(SoftGoalConditions);

        var unsatisfiedConditions = new List<IPartialCondition>();
        foreach (var condition in conditions) {
            var satisfaction = await condition.Evaluate(HypotheticalFacts);
            if (satisfaction < 1f) {
                unsatisfiedConditions.Add(condition);
            }
        }

        return unsatisfiedConditions;
    }
}