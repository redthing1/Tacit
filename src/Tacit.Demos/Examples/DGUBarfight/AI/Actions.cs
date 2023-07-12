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

    public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff) {
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

    public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff) {
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

    public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff) {
        // if we punch someone, they will get hurt, and probably punch back
        // TODO: implement this
        return Task.FromResult(new List<WorldDiff>());
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