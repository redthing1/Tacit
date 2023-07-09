namespace Tacit.Calc;

public static class IntMath {
    public static int Clamp(int v, int min, int max) {
        if (v < min) return min;
        if (v > max) return max;
        return v;
    }
}