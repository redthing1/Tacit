using System;

namespace Tacit.Framework.Utility.Considerations;

/// <summary>
///     Adds all appraisals. Each must score above the threshold
/// </summary>
/// <typeparam name="T"></typeparam>
public class ThresholdConsideration<T> : Consideration<T> {
    private readonly float _threshold;

    public ThresholdConsideration(Action action, float threshold, string? tag = null) : base(action, tag) {
        _threshold = threshold;
    }

    public override float Score() {
        var sum = 0f;
        foreach (var appraisal in appraisals) {
            var score = ScoreAppraisal(appraisal);
            if (score < _threshold) return 0;
            sum += score;
        }

        return sum;
    }
}