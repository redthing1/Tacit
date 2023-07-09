using System;

namespace Tacit.Calc;

public class Rng {
    private readonly Random _rng;

    public Rng(int seed) {
        _rng = new Random(seed);
    }

    public int Next() {
        return _rng.Next();
    }

    public int Next(int min, int max) {
        return _rng.Next(min, max);
    }

    public float NextFloat() {
        return (float)_rng.NextDouble();
    }
}