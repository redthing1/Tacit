using System;
using System.Collections.Concurrent;
using System.Threading;
using Tacit.Systems;
using Xunit;

namespace Tacit.Tests.Layers.Mind;

public class BasicMind : Mind<BasicMind.State> {
    public BasicMind() : base(new State()) {}

    public override void Initialize() {
        base.Initialize();

        sensorySystems.Add(new RhythmSystem(this, refresh: 0.5f, cancelToken.Token));
        cognitiveSystems.Add(new RandomSeedSystem(this, refresh: 0.5f, cancelToken.Token));
        cognitiveSystems.Add(new TaxReturnsSystem(this, refresh: 0.5f, cancelToken.Token));
    }

    public class State : MindState {
        public ConcurrentQueue<PlanTask> plan = new();
    }

    /// <summary>
    ///     represents a system that tracks beats. every time it processes, it simply increments the beat counter.
    /// </summary>
    public class RhythmSystem : MindSystem<BasicMind, State> {
        public int beats;

        public RhythmSystem(BasicMind mind, float refresh, CancellationToken cancelToken) : base(mind, refresh,
            cancelToken) {}

        override protected void Process() {
            beats++;
        }
    }

    /// <summary>
    ///     represents a system that calculates and stores a random number
    /// </summary>
    public class RandomSeedSystem : MindSystem<BasicMind, State> {
        public Random rng = new();
        public int _rndState = -1;// -1 indicates uninitialized

        public RandomSeedSystem(BasicMind mind, float refresh, CancellationToken cancelToken) : base(mind, refresh,
            cancelToken) {}

        override protected void Process() {
            // this always gives us a nonnegative integer
            _rndState = rng.Next();
        }
    }

    public class TaxReturnsSystem : PlannerSystem<BasicMind, State> {
        public int paid;

        public TaxReturnsSystem(BasicMind mind, float refresh, CancellationToken cancelToken) : base(mind, refresh,
            cancelToken) {}

        override protected bool ProcessSignal(MindSignal signal) {
            if (signal is BillSignal bill) {
                paid += bill.Price;
                return true;
            }

            return false;
        }

        override protected void MakePlans() {
            // nothing to do here
        }
    }

    public class BillSignal : MindSignal {
        public BillSignal(int price) {
            this.Price = price;
        }

        public int Price { get; }
    }

    public class PushButtonTask : PlanTask<BasicMind> {
        public PushButtonTask(BasicMind mind, float expiryTime = 0) : base(mind, expiryTime) {}
        public bool Pressed { get; private set; }

        public override PlanTaskStatus Status() {
            if (base.Status() == PlanTaskStatus.Failed) return PlanTaskStatus.Failed;// check base conditions

            if (Pressed) return PlanTaskStatus.Complete;

            return PlanTaskStatus.Ongoing;
        }

        public void Press() {
            Assert.False(Pressed);
            Pressed = true;
        }
    }
}