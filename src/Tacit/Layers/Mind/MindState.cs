using System.Collections.Concurrent;

namespace Tacit;

/// <summary>
///     represents all data that is considered "state" or "working" information of the mind.
///     it can include things like caches for senses and observations
///     and plans and working memory
/// </summary>
public abstract class MindState {
    public ConcurrentQueue<MindSignal> signalQueue = new();// signals to be processed
    protected int ticks;
    public float totalTime;

    /// <summary>
    ///     called by the Mind, representing elapsed time
    /// </summary>
    /// <param name="elapsed"></param>
    public virtual void Tick(float elapsed) {
        ticks++;
        totalTime += elapsed;
    }
}