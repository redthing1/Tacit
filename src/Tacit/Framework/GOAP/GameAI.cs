using System;
using System.ComponentModel;
using Tacit.Framework.GOAP.Details;
using static Tacit.Framework.GOAP.Details.Util;
using S=Tacit.Framework.GOAP.PlanningState;

namespace Tacit.Framework.GOAP; // See also Runtime/Unity/GameAI.cs)

public abstract class GameAI<T>
    : SolverOwner, INotifyPropertyChanged where T : class {
    public SolverParams config = new();

    //
    public float cooldown;
    private Goal<T>[] goals;

    public ActionHandler handler = new ActionMap();

    //
    private int index;
    public Handlers policies = new();
    public Solver<T> solver;
    public bool verbose;
    public Goal<T> goal => goals[index];
    public S status => solver.status;

    public event PropertyChangedEventHandler PropertyChanged;

    public SolverStats stats => solver;

    public abstract Goal<T>[] Goals();
    public abstract T Model();
    public virtual void Idle() {}
    public virtual bool IsActing() {
        return false;
    }

    public virtual void Update() {
        solver = solver ?? new Solver<T>();
        if (policies.Block(status) || IsActing()) return;
        var s = status;
        var next = solver.status != S.Running
            ? StartSolving()
            : solver.Iterate(config.frameBudget);
        if (next != null) handler.Effect(next.Head(), this);
        policies.OnResult(status, ObjectName(this));
        if (s != status) NotifyPropertyChanged(nameof(status));
    }

    private Node<T> StartSolving() {
        if (handler is ActionMap m) m.verbose = verbose;
        var model = Model();
        if (model == null) return null;
        var goal = NextGoal();
        config.Reset(solver);
        return solver.Next(model, goal, config.frameBudget);
    }

    private Goal<T> NextGoal() {
        goals = Goals();
        switch (status) {
            case S.Done:
                index = 0;
                break;
            case S.Running: throw new Exception("Invalid");
            default:
                index = index + 1;
                if (index >= goals.Length) {
                    index = 0;
#if UNITY_2018_1_OR_NEWER
                Cooldown();
#endif
                }
                break;
        }
        return goals[index];
    }

    private void NotifyPropertyChanged(string p) {
        PropertyChanged?
            .Invoke(this, new PropertyChangedEventArgs(p));
    }
}