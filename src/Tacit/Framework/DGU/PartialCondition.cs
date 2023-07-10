using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public abstract class PartialCondition {
    public AgentEnvironment Environment { get; }
    public FactChange SatisfactionCriterion { get; init; } = null!;

    public PartialCondition(AgentEnvironment environment) {
        Environment = environment;
    }

    public abstract Task<float> Evaluate();
}