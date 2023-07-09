using Tacit.Framework.Utility;
using Tacit.Framework.Utility.Considerations;
using Xunit;

namespace Tacit.Tests.Framework.Utility;

public class AppraisalTests {
    [Fact]
    public void CanTransformAppraisals() {
        var carp1 = new MagicCarpet(balloons: 6, weights: 0);

        // raw score
        var basic = new BalloonAppraisal(carp1);
        Assert.Equal(expected: 0.6f, basic.TransformedScore(), precision: 4);

        var clamped = new BalloonAppraisal(carp1).Clamp(0.3f);
        Assert.Equal(expected: 0.3f, clamped.TransformedScore(), precision: 4);

        var scaled = new BalloonAppraisal(carp1).Scale(0.1f);
        Assert.Equal(expected: 0.06f, scaled.TransformedScore(), precision: 4);

        var inverted = new BalloonAppraisal(carp1).Inverse();
        Assert.Equal(expected: 0.4f, inverted.TransformedScore(), precision: 4);

        var negated = new BalloonAppraisal(carp1).Negate();
        Assert.Equal(expected: -0.6f, negated.TransformedScore(), precision: 4);

        var clampedNegated = new BalloonAppraisal(carp1).Clamp(0.3f).Negate();
        Assert.Equal(expected: -0.3f, clampedNegated.TransformedScore(), precision: 4);
    }

    [Fact]
    public void CanScoreConsideration() {
        var carpet = new MagicCarpet(balloons: 2, weights: 2);
        var floatConsid = new ThresholdSumConsideration<MagicCarpet>(action: () => {}, threshold: 0.5f);
        floatConsid.AddAppraisal(new BalloonAppraisal(carpet));
        floatConsid.AddAppraisal(new WeightsAppraisal(carpet).Negate());

        var score = floatConsid.Score();
        Assert.Equal(expected: 0f, score, precision: 4);
    }

    [Fact]
    public void CanScoreConsiderationsWithTransformedAppraisals() {
        var carpet = new MagicCarpet(balloons: 4, weights: 8);
        var floatConsid = new SumConsideration<MagicCarpet>(() => {});
        floatConsid.AddAppraisal(new BalloonAppraisal(carpet));
        floatConsid.AddAppraisal(new WeightsAppraisal(carpet).Clamp(0.6f).Negate());

        var score = floatConsid.Score();
        Assert.Equal(expected: -0.2f, score, precision: 4);
    }

    private class MagicCarpet {
        /// <summary>
        ///     the maximum amount of force able to be applied by balloons or weights
        /// </summary>
        public const int MAX_FORCE = 10;

        /// <summary>
        ///     balloons make the magic carpet rise
        /// </summary>
        public readonly int balloons;

        /// <summary>
        ///     weights make the magic carpet sink
        /// </summary>
        public readonly int weights;

        public MagicCarpet(int balloons, int weights) {
            this.balloons = balloons;
            this.weights = weights;
        }
    }

    private class BalloonAppraisal : Appraisal<MagicCarpet> {
        public BalloonAppraisal(MagicCarpet context) : base(context) {}

        public override float Score() {
            return (float)context.balloons / MagicCarpet.MAX_FORCE;
        }
    }

    private class WeightsAppraisal : Appraisal<MagicCarpet> {
        public WeightsAppraisal(MagicCarpet context) : base(context) {}

        public override float Score() {
            return (float)context.weights / MagicCarpet.MAX_FORCE;
        }
    }
}