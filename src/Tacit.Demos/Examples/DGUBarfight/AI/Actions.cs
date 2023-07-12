using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tacit.Framework.DGU;
using Tacit.Primer;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class GetMoreDrunkEffect : VirtualEffect {
    public GetMoreDrunkEffect(ISmartObject supplier) : base(
        new FactChange(
            supplier.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Increase)
    ) {
    }

    public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff, IReadOnlyFactMemory memory) {
        // nothing much will happen
        return Task.FromResult(new List<WorldDiff>());
    }
}

public class GetLessDrunkEffect : VirtualEffect {
    public GetLessDrunkEffect(ISmartObject supplier) : base(
        new FactChange(
            supplier.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Decrease)
    ) {
    }

    public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff, IReadOnlyFactMemory memory) {
        // nothing much will happen
        return Task.FromResult(new List<WorldDiff>());
    }
}

public class PunchSomeoneEffect : VirtualEffect {
    public ISmartObject Supplier { get; }
    public ISmartObject Consumer { get; }

    public PunchSomeoneEffect(ISmartObject supplier, ISmartObject consumer) : base(
        new FactChange(
            supplier.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Decrease)
    ) {
        Supplier = supplier;
        Consumer = consumer;
    }

    public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff, IReadOnlyFactMemory memory) {
        // if we punch someone, they will get hurt, and probably punch back
        var outDiffs = new List<WorldDiff>();
        
        // if this diff is the supplier's health decreasing, then the supplier will also punch back at us (the consumer)
        if (diff.depth == 0) { // the seed diff
            // ensure
            if (diff.Fact.SubjectId != Supplier.Id) {
                throw new InvalidOperationException($"PunchSomeoneEffect: diff.Fact.SubjectId != Supplier.Id");
            }
            // first, decrease the supplier's health cause they were punched
            var supplierHealthFact = diff.Fact as Fact<float>;
            var newSupplierHealth = Mathf.Clamp(supplierHealthFact!.Value - Constants.Values.PUNCH_DAMAGE, 0, Constants.Values.HEALTH_MAX);
            var newSupplierHealthFact = new Fact<float>(Supplier, Constants.Facts.PERSON_HEALTH, newSupplierHealth,
                supplierHealthFact.Time + 1);
            var supplierHealthDecrease =
                new FactChange(Supplier.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Decrease);
            outDiffs.Add(new WorldDiff(supplierHealthDecrease, newSupplierHealthFact, diff.depth + 1));
            
            // now, the supplier will punch back at us
            var consumerHealthDecrease =
                new FactChange(Consumer.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Decrease);
            var consumerHealthFact = memory.ExpectFact<float>(Consumer.Id, Constants.Facts.PERSON_HEALTH);
            var newConsumerHealth = Mathf.Clamp(consumerHealthFact.Value - Constants.Values.PUNCH_DAMAGE, 0, Constants.Values.HEALTH_MAX);
            var newConsumerHealthFact = new Fact<float>(Consumer, Constants.Facts.PERSON_HEALTH, newConsumerHealth,
                consumerHealthFact.Time + 1);
            outDiffs.Add(new WorldDiff(consumerHealthDecrease, newConsumerHealthFact, diff.depth + 1));
        }
        
        return Task.FromResult(outDiffs);
    }
}

public class DrinkAlcoholAction : VirtualAction {
    public DrinkAlcoholAction(ISmartObject supplier, ISmartObject consumer) : base(supplier, consumer) {
        // // don't drink if way too drunk
        // Preconditions.Add(new FuncPartialCondition(
        //     new FactChange(supplier.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Decrease), async mem => {
        //         var drunkennessFact = mem.ExpectFact<float>(supplier.Id, Constants.Facts.PERSON_DRUNKENNESS);
        //         return Mathf.Map01Clamp01(drunkennessFact.Value, 0, Constants.Values.DANGEROUS_DRUNKENNESS);
        //     }));

        Effects.Add(new GetMoreDrunkEffect(supplier));
    }
}

public class EatBreadAction : VirtualAction {
    public EatBreadAction(ISmartObject supplier, ISmartObject consumer) : base(supplier, consumer) {
        Effects.Add(new GetLessDrunkEffect(supplier));
    }
}

public class ThrowPunchAction : VirtualAction {
    public ThrowPunchAction(ISmartObject supplier, ISmartObject consumer) : base(supplier, consumer) {
        // can't punch if too drunk
        Preconditions.Add(new FuncPartialCondition(
            new FactChange(supplier.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Decrease), async mem => {
                var drunkennessFact = mem.ExpectFact<float>(supplier.Id, Constants.Facts.PERSON_DRUNKENNESS);
                return 1 - Mathf.Map01Clamp01(drunkennessFact.Value, 0, Constants.Values.DRUNKENNESS_IMPAIRED);
            }));
        Effects.Add(new PunchSomeoneEffect(supplier, consumer));
    }
}