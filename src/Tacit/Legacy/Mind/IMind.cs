namespace Tacit.Legacy.Mind;

public interface IMind {
    /// <summary>
    ///     the number of ticks
    /// </summary>
    int Ticks { get; }

    /// <summary>
    ///     the elapsed time within this mind
    /// </summary>
    float Elapsed { get; }

    /// <summary>
    ///     allocate resources and start
    /// </summary>
    void Initialize();

    /// <summary>
    ///     deallocate resources and halt
    /// </summary>
    void Destroy();

    /// <summary>
    ///     step all the systems (main thread)
    /// </summary>
    /// <param name="dt"></param>
    void Tick(float dt);

    /// <summary>
    ///     propagate a signal
    /// </summary>
    /// <param name="signal"></param>
    void Signal(MindSignal signal);
}