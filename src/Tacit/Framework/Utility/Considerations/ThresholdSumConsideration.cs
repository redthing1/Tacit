using System;

namespace Tacit.Framework.Utility.Considerations;

/// <summary>
///     Adds all appraisals. The total must score above the threshold
/// </summary>
/// <typeparam name="T"></typeparam>
public class ThresholdSumConsideration<T> : Consideration<T> {
    private readonly float _threshold;

    public ThresholdSumConsideration(Action action, float threshold, string? tag = null) : base(action, tag) {
        _threshold = threshold;
    }

    public override float Score() {
        var sum = 0f;
        foreach (var appraisal in appraisals) {
            sum += ScoreAppraisal(appraisal);
        }

        if (sum < _threshold) sum = 0;
        return sum;
    }
}