namespace Tacit;

public enum PlanTaskStatus {
    /// <summary>
    ///     task is still running
    /// </summary>
    Ongoing,

    /// <summary>
    ///     task completed successfully
    /// </summary>
    Complete,

    /// <summary>
    ///     task failed, but is optional
    /// </summary>
    OptionalFailed,

    /// <summary>
    ///     task failed unrecoverably
    /// </summary>
    Failed
}

public abstract class PlanTask {
    public abstract PlanTaskStatus Status();
}

public abstract class PlanTask<TMind> : PlanTask where TMind : IMind {
    protected readonly TMind mind;
    public float expiryTime;

    public PlanTask(TMind mind, float expiryTime = 0f) {
        this.mind = mind;
        this.expiryTime = expiryTime;
    }

    /// <summary>
    ///     whether the goal should still be pursued (valid/ongoing)
    /// </summary>
    /// <returns></returns>
    public override PlanTaskStatus Status() {
        if (expiryTime <= 0) return PlanTaskStatus.Ongoing;
        return mind.Elapsed < expiryTime ? PlanTaskStatus.Ongoing : PlanTaskStatus.Failed;
    }
}