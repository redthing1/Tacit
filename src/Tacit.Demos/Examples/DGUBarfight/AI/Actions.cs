using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tacit.Framework.DGU;
using Tacit.Primer;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class PunchSomeoneEffect : VirtualEffect {
    public PunchSomeoneEffect(VirtualAction cause) : base(
        new FactChange(
            cause.Supplier!.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Decrease), cause
    ) {
    }

    public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff, IReadOnlyFactMemory memory) {
        // if we punch someone, they will get hurt, and probably punch back
        var outDiffs = new List<WorldDiff>();

        // if this diff is the supplier's health decreasing, then the supplier will also punch back at us (the consumer)
        if (diff.depth == 0) {
            // the seed diff
            // ensure
            if (diff.Fact.SubjectId != CauseAction!.Supplier!.Id) {
                throw new InvalidOperationException($"PunchSomeoneEffect: diff.Fact.SubjectId != Supplier.Id");
            }

            // first, decrease the supplier's health cause they were punched
            var supplierHealthFact = diff.Fact as Fact<float>;
            var newSupplierHealth = Mathf.Clamp(supplierHealthFact!.Value - Constants.Values.BASE_PUNCH_DAMAGE, 0,
                Constants.Values.HEALTH_MAX);
            var newSupplierHealthFact = new Fact<float>(CauseAction!.Supplier!, Constants.Facts.PERSON_HEALTH,
                newSupplierHealth,
                supplierHealthFact.Time + 1);
            var supplierHealthDecrease =
                new FactChange(CauseAction!.Supplier!.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Decrease);
            outDiffs.Add(new WorldDiff(supplierHealthDecrease, newSupplierHealthFact, diff.depth + 1));

            // now, the supplier will punch back at us
            var consumerHealthDecrease =
                new FactChange(CauseAction!.Supplier!.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Decrease);
            var consumerHealthFact = memory.ExpectFact<float>(CauseAction!.Consumer!.Id, Constants.Facts.PERSON_HEALTH);
            var newConsumerHealth = Mathf.Clamp(consumerHealthFact.Value - Constants.Values.BASE_PUNCH_DAMAGE, 0,
                Constants.Values.HEALTH_MAX);
            var newConsumerHealthFact = new Fact<float>(CauseAction!.Consumer!, Constants.Facts.PERSON_HEALTH,
                newConsumerHealth,
                consumerHealthFact.Time + 1);
            outDiffs.Add(new WorldDiff(consumerHealthDecrease, newConsumerHealthFact, diff.depth + 1));
        }

        return Task.FromResult(outDiffs);
    }
}

public class GetMoreDrunkEffect : VirtualEffect {
    public GetMoreDrunkEffect(VirtualAction cause) : base(
        new FactChange(
            cause!.Supplier!.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Increase),
        cause
    ) {
    }

    public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff, IReadOnlyFactMemory memory) {
        var outDiffs = new List<WorldDiff>();
        if (diff.depth != 0) return Task.FromResult(outDiffs);

        var supplierDrunkennessFact = diff.Fact as Fact<float>;
        var additionalBac = DrinkCalculator.CalculateDrinkBAC(Constants.Values.TYPICAL_GLASS_VOLUME,
            ((DrinkAlcoholAction)CauseAction!).AlcoholStrength);
        var newSupplierDrunkenness = Mathf.Clamp(supplierDrunkennessFact!.Value + additionalBac, 0,
            Constants.Values.DEADLY_DRUNKENNESS);
        var newSupplierDrunkennessFact = new Fact<float>(CauseAction!.Supplier!, Constants.Facts.PERSON_DRUNKENNESS,
            newSupplierDrunkenness, supplierDrunkennessFact.Time + 1);
        var supplierDrunkennessIncrease =
            new FactChange(CauseAction!.Supplier!.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Increase);
        outDiffs.Add(new WorldDiff(supplierDrunkennessIncrease, newSupplierDrunkennessFact, diff.depth + 1));

        return Task.FromResult(outDiffs);
    }
}

public class GetLessDrunkEffect : VirtualEffect {
    public GetLessDrunkEffect(VirtualAction cause) : base(
        new FactChange(
            cause!.Supplier!.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Decrease),
        cause
    ) {
    }

    public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff, IReadOnlyFactMemory memory) {
        var outDiffs = new List<WorldDiff>();
        if (diff.depth != 0) return Task.FromResult(outDiffs);

        var supplierDrunkennessFact = diff.Fact as Fact<float>;
        var newSupplierDrunkenness =
            Mathf.Clamp(supplierDrunkennessFact!.Value - Constants.Values.BASE_BREAD_BAC_REDUCTION, 0,
                Constants.Values.DEADLY_DRUNKENNESS);
        var newSupplierDrunkennessFact = new Fact<float>(CauseAction!.Supplier!, Constants.Facts.PERSON_DRUNKENNESS,
            newSupplierDrunkenness, supplierDrunkennessFact.Time + 1);
        var supplierDrunkennessDecrease =
            new FactChange(CauseAction!.Supplier!.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Decrease);
        outDiffs.Add(new WorldDiff(supplierDrunkennessDecrease, newSupplierDrunkennessFact, diff.depth + 1));

        return Task.FromResult(outDiffs);
    }
}

public class HealFromAlcoholEffect : VirtualEffect {
    public HealFromAlcoholEffect(VirtualAction cause) : base(
        new FactChange(
            cause!.Supplier!.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Increase),
        cause
    ) {
    }

    public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff, IReadOnlyFactMemory memory) {
        var outDiffs = new List<WorldDiff>();
        if (diff.depth != 0) return Task.FromResult(outDiffs);

        var supplierHealthFact = diff.Fact as Fact<float>;
        var newSupplierHealth = Mathf.Clamp(supplierHealthFact!.Value + Constants.Values.HEAL_FROM_DRINKING_GLASS, 0,
            Constants.Values.HEALTH_MAX);
        var newSupplierHealthFact = new Fact<float>(CauseAction!.Supplier!, Constants.Facts.PERSON_HEALTH,
            newSupplierHealth, supplierHealthFact.Time + 1);
        var supplierHealthIncrease =
            new FactChange(CauseAction!.Supplier!.Id, Constants.Facts.PERSON_HEALTH, FactChangeType.Increase);
        outDiffs.Add(new WorldDiff(supplierHealthIncrease, newSupplierHealthFact, diff.depth + 1));

        return Task.FromResult(outDiffs);
    }
}

public class DrinkAlcoholAction : VirtualAction {
    /// <summary>
    /// the strength of the alcohol in this drink (in %ABV)
    /// </summary>
    public float AlcoholStrength { get; }

    public DrinkAlcoholAction(ISmartObject supplier, ISmartObject consumer, float alcoholStrength) : base(supplier,
        consumer) {
        AlcoholStrength = alcoholStrength;
        Effects.Add(new GetMoreDrunkEffect(this));
        Effects.Add(new HealFromAlcoholEffect(this));
    }
}

public class EatBreadAction : VirtualAction {
    public EatBreadAction(ISmartObject supplier, ISmartObject consumer) : base(supplier, consumer) {
        Effects.Add(new GetLessDrunkEffect(this));
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
        Effects.Add(new PunchSomeoneEffect(this));
    }
}