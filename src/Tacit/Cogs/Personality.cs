using Tacit.Calc;
using Tacit.Primer;

namespace Tacit.Cogs;

public abstract class Personality {
    public abstract float[] Vec { get; }

    /// <summary>
    ///     dot product of normalized vec and weights
    /// </summary>
    /// <param name="weights"></param>
    /// <returns></returns>
    public float Mult(float[] weights) {
        var nvec = Mathf.NormVec(Vec);// normalized personality vector
        return Mathf.Dot(nvec, weights);
    }

    /// <summary>
    ///     dot product of raw vec and weights, without normalization
    /// </summary>
    /// <param name="weights"></param>
    /// <returns></returns>
    public float MultRaw(float[] weights) {
        return Mathf.Dot(Vec, weights);
    }

    public static float NormalRand(float u, float s) {
        return Mathf.Clamp(Distribution.NormalRand(u, s), min: -1f, max: 1f);
    }
}