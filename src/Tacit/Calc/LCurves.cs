using Tacit.Primer;

namespace Tacit.Calc;

public static class LCurves {
    /// <summary>
    ///     x^n function, but absolute value on negative x
    /// </summary>
    /// <param name="v"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static float SymmetricPow(float v, float e) {
        if (v >= 0) return Mathf.Pow(v, e);
        return -Mathf.Abs(Mathf.Pow(v, e));
    }

    /// <summary>
    ///     the advantage [-1,1] given a ratio [0, inf), where <1 is advantage
    /// </summary>
    /// <param name="ratio"></param>
    /// <param name="tightness"></param>
    /// <returns></returns>
    public static float RatioAdvantage(float ratio, float tightness) {
        // this assumes that <1 is advantage and >1 is disadvantage
        // adv(x) = f_rdw(log_h(x))
        var loghx = Mathf.Log(ratio, tightness);
        // f_rdw(x) = 2* ((1)/(1+ ( 1*e^(1x) )  ))
        var fdrw = 2 * (1 / (1 + 1 * Mathf.Exp(1 * loghx))) - 1;

        return fdrw;
    }

    /// <summary>
    ///     models a diminishing return function f(x)=A*e^(-rx), outputs [0,1] given an input value x
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="falloff"></param>
    /// <returns></returns>
    public static float DiminishingReturns(float x, float baseValue, float falloff) {
        return baseValue * Mathf.Exp(-falloff * x);
    }
}