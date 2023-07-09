using System;
using System.Collections.Generic;
using Tacit.Framework.Utility.Considerations;

namespace Tacit.Framework.Utility;

public class Reasoner<T> {
    public enum ScoreType {
        /// <summary>
        ///     use raw sum of appraisals as the score
        /// </summary>
        Raw,

        /// <summary>
        ///     normalize the sum of the appraisals to the interval [0, 1]
        /// </summary>
        Normalized
    }

    private static readonly Random random = new();

    /// <summary>
    ///     whether to enable internal tracing
    /// </summary>
    public static bool trace = false;

    protected List<Consideration<T>> considerations = new();
    public ScoreType scoreType = ScoreType.Raw;

    public void AddConsideration(Consideration<T> consideration) {
        considerations.Add(consideration);
    }

    private float GetScore(Consideration<T> consideration) {
        switch (scoreType) {
            case ScoreType.Raw: return consideration.Score();
            case ScoreType.Normalized: return consideration.NormalizedScore();
        }

        return 0;
    }

    public Dictionary<Consideration<T>, float> Execute() {
        var results = new Dictionary<Consideration<T>, float>();
        Execute(results);
        return results;
    }

    /// <summary>
    ///     runs the reasoner, and stores the results in the provided dictionary
    /// </summary>
    /// <param name="results"></param>
    public void Execute(Dictionary<Consideration<T>, float> results) {
        results.Clear();
        foreach (var consideration in considerations) {
            var score = GetScore(consideration);
            results[consideration] = score;
        }
    }

    public Consideration<T> Choose() {
        var results = Execute();
        return Choose(results);
    }

    public Consideration<T> Choose(Dictionary<Consideration<T>, float> results) {
        var max = default(KeyValuePair<Consideration<T>, float>);
        foreach (var result in results) {
            if (max.Key == null || result.Value > max.Value) {
                max = result;
            }
        }

        return max.Key;
    }

    // public Consideration<T> chooseFuzzy(float fuzzy) {
    //     var results = execute();
    //     var candidates = results.OrderByDescending(x => x.Item2).Take((int) Math.Ceiling(fuzzy * results.Count)).ToList();
    //     return candidates[(int) (random.NextDouble() * candidates.Count)].Item1;
    // }
}