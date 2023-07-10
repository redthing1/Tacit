using System.Collections.Generic;
using System.Threading;

namespace Tacit.Legacy.Mind;

/// <summary>
///     represents the consciousness of a Wing.
///     it can fully control the thoughts and actions of a bird.
/// </summary>
public abstract class Mind<TState> : IMind where TState : MindState, new() {
    // - state
    public readonly TState state;

    private System.Threading.Tasks.Task? _consciousnessTask;

    /// <summary>
    ///     internally used to track consciousness updates only when threadpool is disabled
    /// </summary>
    private float _nextSyncConsciousness;

    /// <summary>
    ///     the cancellation token used for all asynchronous tasks
    /// </summary>
    protected CancellationTokenSource cancelToken;

    public List<IMindSystem> cognitiveSystems = new();

    /// <summary>
    ///     how often to sleep between consciousness updates
    /// </summary>
    public int consciousnessSleep = 100;

    // - systems
    public List<IMindSystem> sensorySystems = new();

    public Mind(TState state) {
        this.state = state;
        cancelToken = new CancellationTokenSource();
    }

    // - options
    public static bool UseThreadPool { get; set; } = true;

    public int Ticks { get; private set; }

    public float Elapsed { get; private set; }

    public virtual void Initialize() {
        // start processing tasks
        if (UseThreadPool) {
            _consciousnessTask =
                System.Threading.Tasks.Task.Run(function: async () => await ConsciousnessAsync(cancelToken.Token), cancelToken.Token);
        }
    }

    public virtual void Destroy() {
        // stop processing tasks
        if (_consciousnessTask != null) cancelToken!.Cancel();
    }

    public void Tick(float dt) {
        Elapsed += dt;
        Ticks++;

        // Sense-Think-Act AI

        // AUTONOMOUS pipeline - sense
        Sense();// sense the world around

        // if thread-pooled AI is disabled, do synchronous consciousness
        // this runs the CONSCIOUS pipeline on the AUTONOMOUS pipeline's thread
        if (!UseThreadPool && _consciousnessTask == null) {
            if (Elapsed >= _nextSyncConsciousness) ConsciousnessStep();
            _nextSyncConsciousness = consciousnessSleep / 1000f;
        }

        // AUTONOMOUS pipeline - act
        Act();// carry out decisions;

        // update state information
        state.Tick(dt);
    }

    public void Signal(MindSignal signal) {
        state.signalQueue.Enqueue(signal);
    }

    /// <summary>
    ///     step the consciousness. one unit of thinking.
    /// </summary>
    private void ConsciousnessStep() {
        Think();
    }

    private async System.Threading.Tasks.Task ConsciousnessAsync(CancellationToken tok) {
        while (!tok.IsCancellationRequested) {
            ConsciousnessStep();

            await System.Threading.Tasks.Task.Delay(consciousnessSleep, tok);
        }
    }

    protected virtual void Act() {}

    protected virtual void Think() {
        // think based on information and make plans
        foreach (var system in cognitiveSystems) {
            system.Tick();
        }
    }

    private void Sense() {
        foreach (var system in sensorySystems) {
            system.Tick();
        }
    }
}