using Tacit.Framework.GOAP;
using Tacit.Framework.GOAP.Details;

namespace Tacit.Tests.Layers.Plan;

// planning model using more verbose, original XGOAP type class
public class BasicRoomCleaner : ActionPlanningModel<BasicRoomCleaner> {
    // - const
    public const int RELAX_COST = 1;
    public const int CLEAN_COST = 10;

    public const int MAX_MESSY = 6;

    // - state
    public int WorkDone { get; set; }
    public int Messiness { get; set; }

    override protected Option[] ActionOptions => new Option[] { Relax, CleanRoom };

    public Cost Relax() {
        if (Messiness >= MAX_MESSY) return false;
        WorkDone += 1;
        return RELAX_COST;
    }

    public Cost CleanRoom() {
        if (Messiness == 0) return false;
        Messiness = 0;
        return CLEAN_COST;
    }

    public override BasicRoomCleaner Clone(BasicRoomCleaner b) {
        b.WorkDone = WorkDone;
        b.Messiness = Messiness;

        return b;
    }

    public override bool Equals(BasicRoomCleaner b) {
        return b.WorkDone == WorkDone && b.Messiness == Messiness;
    }

    public override int GetHashCode() {
        return (workDone: WorkDone, messiness: Messiness).GetHashCode();
    }
}

// planning model using more concise, less boilerplate smart action planning model
public class SmartRoomCleaner : SmartActionPlanningModel<SmartRoomCleaner> {
    // - const
    public const int RELAX_COST = 1;
    public const int CLEAN_COST = 10;

    public const int MAX_MESSY = 6;

    // - state
    public int WorkDone { get; set; }
    public int Messiness { get; set; }

    override protected Option[] ActionOptions => new Option[] { Relax, CleanRoom };

    public Cost Relax() {
        if (Messiness >= MAX_MESSY) return false;
        WorkDone += 1;
        return RELAX_COST;
    }

    public Cost CleanRoom() {
        if (Messiness == 0) return false;
        Messiness = 0;
        return CLEAN_COST;
    }
}