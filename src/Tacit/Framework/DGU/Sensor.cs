using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public abstract class Sensor : IDGUDoctorable {
    private readonly AgentEnvironment _env;
    private readonly FactMemory _factMemory;
    public DGUDoctor? Doctor { get; set; }

    public Sensor(AgentEnvironment env, FactMemory factMemory) {
        _env = env;
        _factMemory = factMemory;
    }

    public virtual Task Update(long time) {
        return Task.CompletedTask;
    }
}