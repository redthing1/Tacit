using System.Collections.Generic;
using System.Linq;

namespace Tacit.Calc;

public class DiscreteProbabilityDistribution<T> {
    private readonly List<(float, T)> _probabilities;
    private readonly Rng _rng;

    public DiscreteProbabilityDistribution(Rng rng) {
        _rng = rng;
        _probabilities = new List<(float, T)>();
    }

    public DiscreteProbabilityDistribution(Rng rng, (float, T)[] probabilities) {
        _rng = rng;
        _probabilities = probabilities.ToList();
    }

    public void Add(float probability, T outcome) {
        _probabilities.Add((probability, outcome));
    }

    public T Next() {
        var r = _rng.NextFloat();
        var sum = 0f;
        foreach (var (prob, outcome) in _probabilities) {
            if (r >= sum && r < sum + prob) return outcome;

            sum += prob;
        }

        return default!;
    }
}