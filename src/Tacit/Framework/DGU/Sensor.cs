using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public abstract class Sensor {
    private readonly AgentEnvironment _env;
    private readonly FactMemory _factMemory;

    public Sensor(AgentEnvironment env, FactMemory factMemory) {
        _env = env;
        _factMemory = factMemory;
    }

    public virtual Task Update() {
        return Task.CompletedTask;
    }
}