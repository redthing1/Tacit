using System;
using Minlog;
using Tacit.Framework.DGU;

namespace Tacit.Demos.Util;

public class MinlogDGUDoctor : DGUDoctor {
    private ILogger _log;
    private readonly Logger _rootLog;
    private readonly ILogger _forLog;
    public MinlogDGUDoctor(Logger log) {
        _log = log;
        _rootLog = log;
        _forLog = log.For<MinlogDGUDoctor>();
    }

    public override void Log(LogLevel level, string message) {
        switch (level) {
            case LogLevel.Error:
                _log.Err(message);
                break;
            case LogLevel.Warning:
                _log.Warn(message);
                break;
            case LogLevel.Info:
                _log.Info(message);
                break;
            case LogLevel.Trace:
                _log.Trace(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }

    public override void OnAttach(IDGUDoctorable doctored) {
        base.OnAttach(doctored);

        // _log = _log.For<>()
        // invoke Log.For<T>() on the doctored object
        var doctoredObjectType = doctored.GetType();
        var logForMethod = typeof(Logger).GetMethod(nameof(Logger.For));
        var logForGenericMethod = logForMethod?.MakeGenericMethod(doctoredObjectType);
        var logObj = logForGenericMethod?.Invoke(_log, null);
        _log = logObj as ILogger;
        if (_log == null) {
            _forLog.Warn($"Failed to attach doctor to {doctoredObjectType.Name}");
            return;
        }
        _forLog.Info($"Attached doctor to {doctoredObjectType.Name}");
    }
}