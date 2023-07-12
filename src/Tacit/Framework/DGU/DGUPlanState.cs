using System.Collections.Generic;
using System.Threading.Tasks;
using Tacit.Utils;

namespace Tacit.Framework.DGU;

public class DGUPlanState {
    private IdGenerator _idGenerator = new();
    
    public long Id { get; init; }

    /// <summary>
    /// must be satisfied as part of plan formulation. necessary for plan to be valid
    /// </summary>
    public List<IPartialCondition>? HardGoalConditions { get; private set; } = new();

    /// <summary>
    /// desirable to satisfy, but not required. used to calculate state utility
    /// </summary>
    public List<IPartialCondition>? SoftGoalConditions { get; private set; } = new();

    public FactMemory HypotheticalFacts { get; private set; } = null!;

    public DGUPlanState? Parent { get; set; }
    public VirtualAction? ActionGeneratedBy { get; set; } = null!;
    public float Score { get; set; }

    public DGUPlanState(long id, List<IPartialCondition>? hardGoalConditions, List<IPartialCondition>? softGoalConditions, DGUPlanState? parent, FactMemory hypotheticalFacts, VirtualAction? actionGeneratedBy) {
        // if Id is specified, use it. otherwise, generate a new one
        Id = id >= 0 ? id : _idGenerator.GetNextId();
        HardGoalConditions = hardGoalConditions;
        SoftGoalConditions = softGoalConditions;
        Parent = parent;
        HypotheticalFacts = hypotheticalFacts;
        ActionGeneratedBy = actionGeneratedBy;
    }

    public List<VirtualAction> CollectPredecessorActions() {
        // walk up the tree and collect all actions, including the one that generated this state
        var actionPredecessors = new List<VirtualAction>();
        var current = this;
        while (current != null) {
            if (current.ActionGeneratedBy != null) {
                actionPredecessors.Add(current.ActionGeneratedBy);
            }

            current = current.Parent;
        }
        // reverse the list so that the actions are in chronological order
        actionPredecessors.Reverse();

        return actionPredecessors;
    }

    public async Task<List<IPartialCondition>> GetUnsatisfiedPreconditions() {
        var conditions = new List<IPartialCondition>();
        
        if (HardGoalConditions != null) conditions.AddRange(HardGoalConditions);

        var unsatisfiedConditions = new List<IPartialCondition>();
        foreach (var condition in conditions) {
            var satisfaction = await condition.Evaluate(HypotheticalFacts);
            if (satisfaction < 1f) {
                unsatisfiedConditions.Add(condition);
            }
        }

        return unsatisfiedConditions;
    }

    public override string ToString() {
        return $"{GetType().Name}(id={Id}, score={Score})";
    }

    // public DGUPlanState Fork() {
    //     var ret = (DGUPlanState)MemberwiseClone();
    //     // deep copy
    //     ret.HypotheticalFacts = HypotheticalFacts.Fork();
    //     if (HardGoalConditions != null) ret.HardGoalConditions = new List<IPartialCondition>(HardGoalConditions);
    //     if (SoftGoalConditions != null) ret.SoftGoalConditions = new List<IPartialCondition>(SoftGoalConditions);
    //     return ret;
    // }

    public DGUPlanState CreateChildState(VirtualAction creatingAction) {
        var childFacts = HypotheticalFacts.Fork();
        List<IPartialCondition>? hardGoalConditions = null;
        List<IPartialCondition>? softGoalConditions = null;
        if (HardGoalConditions != null) hardGoalConditions = new List<IPartialCondition>(HardGoalConditions);
        if (SoftGoalConditions != null) softGoalConditions = new List<IPartialCondition>(SoftGoalConditions);
        
        var child = new DGUPlanState(_idGenerator.GetNextId(), hardGoalConditions, softGoalConditions, this, childFacts, creatingAction);
        return child;
    }
}