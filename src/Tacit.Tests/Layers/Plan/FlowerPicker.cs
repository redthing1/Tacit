using Tacit.Framework.GOAP;
using Tacit.Framework.GOAP.Details;
using Tacit.Legacy.Plan;

namespace Tacit.Tests.Layers.Plan;

/// <summary>
///     picking flowers
/// </summary>
public class FlowerPicker : SmartActionPlanningModel<FlowerPicker> {
    // - const
    public const int BUCKET_CAPACITY = 4;
    public const int PICK_COST = 1;

    public const int DUMP_COST = 6;

    // - state
    public int FlowersPicked { get; set; }
    public int Bucket { get; set; }

    override protected Option[] ActionOptions => new Option[] { PickFlower, DumpBucket };

    public Cost PickFlower() {
        // precondition
        if (Bucket >= BUCKET_CAPACITY) return false;

        // action
        Bucket++;

        // cost
        return PICK_COST;
    }

    public Cost DumpBucket() {
        // precondition
        if (Bucket == 0) return false;

        // action
        FlowersPicked += Bucket;
        Bucket = 0;

        // cost
        return DUMP_COST;
    }
}