using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public interface IPartialCondition {
    FactChange SatisfactionCriterion { get; set; }
    Task<float> Evaluate(FactMemory facts);
}

public abstract class AbstractPartialCondition : IPartialCondition {
    public FactChange SatisfactionCriterion { get; set; }

    public AbstractPartialCondition(FactChange satisfactionCriterion) {
        SatisfactionCriterion = satisfactionCriterion;
    }

    public abstract Task<float> Evaluate(FactMemory facts);
}

public class FuncPartialCondition : AbstractPartialCondition {
    private Func<FactMemory, Task<float>> _evaluator;

    public FuncPartialCondition(FactChange satisfactionCriterion, Func<FactMemory, Task<float>> evaluator) : base(satisfactionCriterion) {
        _evaluator = evaluator;
    }

    public override Task<float> Evaluate(FactMemory facts) {
        return _evaluator(facts);
    }
}

public abstract class Goal {
    public abstract string Name { get; }
    public abstract long Weight { get; }

    public Drive DriveGeneratedBy { get; init; } = null!;
    public List<IPartialCondition> Conditions { get; } = new();
    public List<GoalTrigger> RemovalTriggers { get; } = new();

    public float CurrentSatisfaction { get; protected set; }

    protected Goal(Drive driveGeneratedBy) {
        DriveGeneratedBy = driveGeneratedBy;
    }

    public async Task Update() {
        CurrentSatisfaction = await Evaluate();
    }

    protected abstract Task<float> Evaluate();
}