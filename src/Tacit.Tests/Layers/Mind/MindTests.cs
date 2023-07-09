using System;
using System.Linq;
using Xunit;

namespace Tacit.Tests.Layers.Mind;

public class MindTests : IDisposable {
    private readonly BasicMind _mind;

    public MindTests() {
        _mind = new BasicMind();
        // use single thread for tests
        BasicMind.UseThreadPool = false;
        _mind.Initialize();
    }

    public void Dispose() {
        _mind.Destroy();
    }

    [Fact]
    public void CanConstructMind() {
        Assert.NotNull(_mind.state);
    }

    [Fact]
    public void CanAddSystems() {
        Assert.NotEmpty(_mind.sensorySystems);
        Assert.NotEmpty(_mind.cognitiveSystems);
    }

    /// <summary>
    ///     ensure that systems can be updated
    /// </summary>
    [Fact]
    public void CanUpdateSystems() {
        _mind.Tick(0.1f);
        var rhythmSystem = (BasicMind.RhythmSystem)_mind.sensorySystems.Single(x => x is BasicMind.RhythmSystem);
        Assert.True(rhythmSystem.beats > 0);
        var rngSystem =
            (BasicMind.RandomSeedSystem)_mind.cognitiveSystems.Single(x => x is BasicMind.RandomSeedSystem);
        Assert.True(rngSystem._rndState >= 0);
    }

    /// <summary>
    ///     ensure that systems don't update faster than their interval
    /// </summary>
    [Fact]
    public void SystemUpdateThrottled() {
        _mind.Tick(0.1f);
        var rhythmSystem = (BasicMind.RhythmSystem)_mind.sensorySystems.Single(x => x is BasicMind.RhythmSystem);
        Assert.True(rhythmSystem.beats == 1);
        _mind.Tick(0.5f);
        Assert.True(rhythmSystem.beats == 2);
    }

    [Fact]
    public void CanPropagateSignals() {
        var taxSystem = (BasicMind.TaxReturnsSystem)_mind.cognitiveSystems.Single(x => x is BasicMind.TaxReturnsSystem);
        Assert.Equal(expected: 0, taxSystem.paid);
        // send a signal
        _mind.Signal(new BasicMind.BillSignal(10));
        // respond to the signal
        _mind.Tick(0.1f);
        Assert.Equal(expected: 10, taxSystem.paid);
    }

    [Fact]
    public void CanCompleteTasks() {
        // add plan to task
        _mind.state.plan.Enqueue(new BasicMind.PushButtonTask(_mind));
        Assert.True(_mind.state.plan.TryPeek(out var task));
        var btnTask = task as BasicMind.PushButtonTask;
        Assert.Equal(PlanTaskStatus.Ongoing, btnTask.Status());
        btnTask.Press();
        Assert.Equal(PlanTaskStatus.Complete, btnTask.Status());
    }

    [Fact]
    public void TasksCanExpire() {
        // add plan to task
        _mind.state.plan.Enqueue(new BasicMind.PushButtonTask(_mind, expiryTime: 1f));
        Assert.True(_mind.state.plan.TryPeek(out var task));
        var btnTask = task as BasicMind.PushButtonTask;
        if (btnTask == null) throw new ArgumentNullException(nameof(btnTask));
        Assert.Equal(PlanTaskStatus.Ongoing, btnTask.Status());
        _mind.Tick(0.5f);
        Assert.Equal(PlanTaskStatus.Ongoing, btnTask.Status());
        _mind.Tick(0.5f);
        Assert.Equal(PlanTaskStatus.Failed, btnTask.Status());
    }
}