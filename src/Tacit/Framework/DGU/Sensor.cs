using System.Threading.Tasks;

namespace Tacit.Framework.DGU;

public abstract class Sensor {
    public DGUAgent Agent { get; }
    public Sensor(DGUAgent agent) {
        Agent = agent;
    }

    public virtual Task Update(long time, FactMemory memory) {
        return Task.CompletedTask;
    }
}