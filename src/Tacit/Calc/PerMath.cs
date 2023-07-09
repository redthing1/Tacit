using Tacit.Primer;

namespace Tacit.Calc;

/// <summary>
///     Personality math utilities
/// </summary>
public static class PerMath {
    /// <summary>
    ///     Given v, take (min * v) and add ((max-min) * d)
    ///     Result is then clamped to [min, max]
    ///     Useful for scaling a value between a min and max value
    /// </summary>
    /// <param name="v"></param>
    /// <param name="d"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float TwoSegment(float v, float d, float min, float max, bool clamp = false) {
        var s1 = v * min;
        var b = max - min;
        var r = s1 + b * d;
        if (clamp) r = Mathf.Clamp(r, min, max);
        return r;
    }

    /// <summary>
    ///     normalize personality component from [-1,1] to [0,1]
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float Map01(float v) {
        return Mathf.Map01(v, min: -1, max: 1);
    }

    /// <summary>
    ///     inverse normalize from [0,1] to [-1,1]
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float Map11(float v) {
        return Mathf.Map(v, leftMin: 0, leftMax: 1, rightMin: -1, rightMax: 1);
    }

    public static float Clamp11(float v) {
        return Mathf.Clamp(v, min: -1, max: 1);
    }
}