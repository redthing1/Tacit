namespace Tacit.Framework.DGU;

/// <summary>
/// used for troubleshooting/logging/debugging of DGU AI
/// </summary>
public abstract class DGUDoctor {
    public enum LogLevel {
        Error = 0,
        Warning = 1,
        Info = 2,
        Trace = 3,
    }

    public abstract void Log(LogLevel level, string message);
    public virtual void OnAttach(IDGUDoctorable doctored) {}
}

public interface IDGUDoctorable {
    public DGUDoctor? Doctor { get; set; }

    public virtual void AttachDoctor(DGUDoctor doctor) {
        Doctor = doctor;
    }
}