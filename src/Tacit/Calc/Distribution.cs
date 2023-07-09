using System.Collections.Generic;
using System.Linq;
using Tacit.Primer;

namespace Tacit.Calc;

public class Distribution {
    public static float ExponentialDf(float x, float m) {
        return Mathf.Exp(-x * m);
    }

    /// <summary>
    ///     selects a random value using a normal distribution
    /// </summary>
    /// <param name="u">mean value</param>
    /// <param name="s">standard deviation</param>
    /// <returns></returns>
    public static float NormalRand(float u, float s) {
        // https://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform
        // https://stackoverflow.com/a/218600
        // i honestly have no idea how this works
        var u1 = 1.0f - Rand.NextFloat();
        var u2 = 1.0f - Rand.NextFloat();
        var dst = Mathf.Sqrt(-2f * Mathf.Log(u1))
                  * Mathf.Sin(2f * Mathf.PI * u2);
        var v =
            u + s * dst;
        return v;
    }

    public static double[] SummarizeDistribution(IEnumerable<double> values) {
        var sorted = values.OrderBy(x => x).ToArray();
        var avg = sorted.Average();
        var min = sorted[0];
        var max = sorted[sorted.Length - 1];
        var q1 = sorted[(int)(sorted.Length * 0.25)];
        var q2 = sorted[(int)(sorted.Length * 0.50)];
        var q3 = sorted[(int)(sorted.Length * 0.75)];
        return new[] { avg, min, q1, q2, q3, max };
    }
}