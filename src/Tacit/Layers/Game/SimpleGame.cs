using System.Threading.Tasks;

namespace Tacit.Layers.Game;

public abstract class SimpleGame {
    public enum Status {
        Unknown,
        Continue,
        Win,
        Lose,
        Error
    }

    public long Steps { get; private set; } = 0;

    public virtual Task<Status> Update() {
        Steps++;
        return Task.FromResult(Status.Unknown);
    }
}