using BenchmarkDotNet.Attributes;
using Tacit.Framework.GOAP;
using Tacit.Framework.GOAP.Details;
using Tacit.Tests.Layers.Plan;

namespace Tacit.Perf.Layers.Plan;

[MemoryDiagnoser]
public class BasicVsSmartPlanner {
    private BasicRoomCleaner _basicCleaner;
    private SmartRoomCleaner _smartCleaner;
    private int _workGoal;

    [Params(100, 1000, 10000)] public int n;

    [GlobalSetup]
    public void Setup() {
        _basicCleaner = new BasicRoomCleaner();
        _smartCleaner = new SmartRoomCleaner();

        _workGoal = n;
    }

    [Benchmark]
    public Node<BasicRoomCleaner>[] SolveBasicCleaner() {
        var solver = new Solver<BasicRoomCleaner>();
        solver.maxIter = solver.maxNodes = _workGoal * 10;// unlock
        var plan = solver.Next(_basicCleaner, new Goal<BasicRoomCleaner>(x => x.WorkDone >= _workGoal));
        return plan.Path();
    }

    [Benchmark]
    public Node<SmartRoomCleaner>[] SolveSmartCleaner() {
        var solver = new Solver<SmartRoomCleaner>();
        solver.maxIter = solver.maxNodes = _workGoal * 10;// unlock
        var plan = solver.Next(_smartCleaner, new Goal<SmartRoomCleaner>(x => x.WorkDone >= _workGoal));
        return plan.Path();
    }
}