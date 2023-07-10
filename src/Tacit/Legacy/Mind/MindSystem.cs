using System.Threading;

namespace Tacit;

public interface IMindSystem {
    bool Tick();
}

/// <summary>
///     Represents a system of a mind with a refresh rate
/// </summary>
public abstract class MindSystem<TMind, TState> : IMindSystem
    where TMind : Mind<TState>
    where TState : MindState, new() {
    protected CancellationToken cancelToken;
    public TMind mind;
    public float nextRefreshAt;
    public float refresh;
    protected TState state;

    public MindSystem(TMind mind, float refresh, CancellationToken cancelToken) {
        this.mind = mind;
        state = mind.state;
        this.refresh = refresh;
        this.cancelToken = cancelToken;
    }

    /// <summary>
    ///     Ticks the mind, calling process if it is time.
    /// </summary>
    /// <returns>Whether process was called.</returns>
    public virtual bool Tick() {
        if (mind.Elapsed >= nextRefreshAt) {
            nextRefreshAt = mind.Elapsed + refresh;
            Process();
            return true;
        }

        return false;
    }

    protected abstract void Process();
}