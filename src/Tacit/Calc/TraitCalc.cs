using Tacit.Primer;

namespace Tacit.Calc;

public static class TraitCalc {
    /// <summary>
    ///     given a trait, maps it between two values and then clamps the result
    /// </summary>
    /// <param name="trait">the trait value [-1, 1]</param>
    /// <param name="mapMin"></param>
    /// <param name="mapMax"></param>
    /// <param name="clampMin"></param>
    /// <param name="clampMax"></param>
    /// <returns></returns>
    public static float Transform(float trait, float mapMin, float mapMax, float clampMin, float clampMax) {
        return Mathf.ClampMapped(trait, modMin: -1f, modMax: 1f, mapMin, mapMax, clampMin, clampMax);
    }

    public static float Transform(float trait, float mapMin, float mapMax) {
        return Transform(trait, mapMin, mapMax, mapMin, mapMax);
    }
}