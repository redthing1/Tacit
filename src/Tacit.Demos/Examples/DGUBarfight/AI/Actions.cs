using System.Collections.Generic;
using System.Threading.Tasks;
using Tacit.Framework.DGU;
using Tacit.Primer;

namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class DrinkAlcoholAction : VirtualAction {
    public override string Name { get; }

    public class GetMoreDrunkEffect : VirtualEffect {
        public GetMoreDrunkEffect(FactChange change) : base(change) {}
        public override Task<List<WorldDiff>> SimulateInWorld(WorldDiff diff) {
            // nothing much will happen
            return Task.FromResult(new List<WorldDiff>());
        }
    }

    public DrinkAlcoholAction(ISmartObject supplier, ISmartObject consumer) : base(supplier, consumer) {
        // don't drink if way too drunk
        Preconditions.Add(new FuncPartialCondition(new FactChange(supplier.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Decrease), async mem => {
            var drunkennessFact = mem.ExpectFact<float>(supplier.Id, Constants.Facts.PERSON_DRUNKENNESS);
            return Mathf.Map01Clamp01(drunkennessFact.Value, 0, Constants.Values.DANGEROUS_DRUNKENNESS);
        }));

        Effects.Add(new GetMoreDrunkEffect(new FactChange(supplier.Id, Constants.Facts.PERSON_DRUNKENNESS, FactChangeType.Increase)));
    }
}