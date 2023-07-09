using System;

namespace Tacit.Framework.Utility.Considerations;

/// <summary>
///     Adds all appraisals
/// </summary>
/// <typeparam name="T"></typeparam>
public class SumConsideration<T> : Consideration<T> {
    public SumConsideration(Action action, string? tag = null) : base(action, tag) {}

    public override float Score() {
        var sum = 0f;
        foreach (var appraisal in appraisals) {
            sum += ScoreAppraisal(appraisal);
        }

        return sum;
    }
}