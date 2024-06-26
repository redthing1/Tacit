using Tacit.Framework.GOAP.Details;
using NullRef=System.NullReferenceException;
using Ex=System.Exception;
using InvOp=System.InvalidOperationException;
using S=Tacit.Framework.GOAP.PlanningState;
using static Tacit.Framework.GOAP.Details.Strings;

namespace Tacit.Framework.GOAP; 

public class Solver<T> : SolverStats where T : class {
    private readonly NodeSet<T> avail = new();
    public bool brfs, safe = true;
    private Dish<T> dish;
    private Goal<T> goal;
    private T initialState;

    public int maxNodes = 1000, maxIter = 1000;
    public float tolerance;

    public bool isRunning => status == S.Running;
    public PlanningState status { get; private set; }
    public int peak { get; private set; }
    public int iteration { get; private set; }

    public Node<T> Next(T s, in Goal<T> goal, int cap = -1) {
        if (s == null) throw new NullRef(NO_INIT_ERR);
        dish = dish ?? Dish<T>.Create(s, safe);
        initialState = s;
        this.goal = goal;
        iteration = 0;
        avail.Init(s, goal.h, !brfs, maxNodes, tolerance);
        return Iterate(cap);
    }

    public Node<T> Iterate(int cap = -1) {
        if (initialState == null) throw new InvOp(NO_INIT_ERR);
        if (status == S.MaxIterExceeded) return null;
        if (cap == -1) cap = maxIter;
        var i = 0;
        while (avail && i++ < cap && iteration++ < maxIter) {
            var current = avail.Pop();
            if (goal.match(current.state)) {
                status = S.Done;
                avail.Clear();
                return current;
            }
            ExpandActions(current, avail);
            ExpandMethods(current, avail);
            if (avail.count > peak) peak = avail.count;
        }
        status = avail.capacityExceeded
            ? S.CapacityExceeded
            : status = avail
                ? iteration < maxIter ? S.Running : S.MaxIterExceeded
                : S.Failed;
        if (status != S.Running) avail.Clear();
        return null;
    }

    public void Reset() {
        avail.Clear();
        status = S.Done;
    }

    private void ExpandActions(Node<T> x, NodeSet<T> @out) {
        if (!(x.state is Agent p)) return;
        var actions = p.Options();
        var count = actions?.Length ?? 0;
        if (count == 0) return;
        dish.Init(x.state);
        T y = null;
        for (var i = 0; i < count; i++) {
            y = dish.Avail();
            var q = y as Agent;
            var r = q.Options()[i]();
            if (r.done) {
                dish.Invalidate();
                if (!brfs && r.cost <= 0) throw new Ex(ZERO_COST_ERR);
                if (@out.Insert(new Node<T>(
                        actions[i], y, x, r.cost))) {
                    dish.Consume();
                }
            }
        }
    }

    // TODO: following current conventions should probably be
    // expandMappedOptions
    private void ExpandMethods(Node<T> x, NodeSet<T> @out) {
        if (!(x.state is Mapped p)) return;
        var count = p.Options()?.Length ?? 0;
        if (count == 0) return;
        dish.Init(x.state);
        T y = null;
        for (var i = 0; i < count; i++) {
            y = dish.Avail();
            var q = y as Mapped;
            var r = q.Options()[i].option();
            if (r.done) {
                dish.Invalidate();
                if (!brfs && r.cost <= 0) {
                    throw new Ex(ZERO_COST_ERR);
                }
                var effect = p.Options()[i].action;
                if (@out.Insert(new Node<T>(effect, y, x, r.cost))) {
                    dish.Consume();
                }
            }
        }
    }
}