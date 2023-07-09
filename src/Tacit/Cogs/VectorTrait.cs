using Tacit.Primer;

namespace Tacit.Cogs;

public static class VectorTrait {
    /// <summary>
    ///     calculate the value of a trait for a personality
    ///     normalizes both the weight vector and the personality vector
    /// </summary>
    /// <param name="weights">trait weights</param>
    /// <param name="p">personality</param>
    /// <returns></returns>
    public static float NormalizedValue(float[] weights, Personality p) {
        var normalizedWeights = Mathf.NormVec(weights);// normalized weight vec
        var result = p.Mult(normalizedWeights);
        return result;
    }

    /// <summary>
    ///     computes the raw dot product of the weight vector and the personality vector
    ///     without normalization
    /// </summary>
    /// <param name="weights"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static float RawValue(float[] weights, Personality p) {
        return p.MultRaw(weights);
    }

    /// <summary>
    ///     second (or higher) order trait calculation
    /// </summary>
    /// <param name="weights"></param>
    /// <param name="traits"></param>
    /// <returns></returns>
    public static float Trait2(float[] weights, float[] traits) {
        var normalizedWeights = Mathf.NormVec(weights);// normalize weights
        var normalizedTraits = Mathf.NormVec(traits);// normalize trait combination
        var result = Mathf.Dot(normalizedWeights, normalizedTraits);
        return result;
    }
}