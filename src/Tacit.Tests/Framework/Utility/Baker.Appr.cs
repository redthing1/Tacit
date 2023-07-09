using Tacit.Framework.Utility;
using Tacit.Primer;

namespace Tacit.Tests.Framework.Utility;

public partial class Baker {
    /// <summary>
    ///     how sleepy we are
    /// </summary>
    private class Sleepy : Appraisal<CakeGame> {
        public Sleepy(CakeGame context) : base(context) {}

        public override float Score() {
            return Mathf.Pow(context.fatigue, y: 0.6f);
        }
    }

    /// <summary>
    ///     how much pending work we have
    /// </summary>
    private class Backlogged : Appraisal<CakeGame> {
        public Backlogged(CakeGame context) : base(context) {}

        public override float Score() {
            // the point where we consider ourself fully backlogged
            var bigBacklog = 4f;
            var backlogProportion = context.orders / bigBacklog;
            // clamp
            return Mathf.Clamp01(backlogProportion);
        }
    }

    /// <summary>
    ///     how badly we need to refill flour
    /// </summary>
    private class LowFlour : Appraisal<CakeGame> {
        public LowFlour(CakeGame context) : base(context) {}

        public override float Score() {
            // how many cakes we can bake with the remaining flour
            var bakeableCakes = context.flour / CakeGame.FLOUR_PER_CAKE;
            // where we're really getting close to running out
            var superLow = 2f;
            if (bakeableCakes <= superLow) return 1f;// max activation
            // distance  to super low
            var dist = bakeableCakes - superLow;
            // the further we are, the lower the activation 
            var distProp = 1f / dist;
            // scale on square root curve
            return Mathf.Pow(distProp, y: 0.5f);
        }
    }
}