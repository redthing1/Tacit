using System;
using System.Collections.Generic;

namespace Tacit.Framework.Utility.Considerations;

public abstract class Consideration<T> {
    public Action action;
    protected List<Appraisal<T>> appraisals = new();
    public Dictionary<Appraisal<T>, float> lastScores = new();

    public Consideration(Action action, string? tag = null) {
        this.action = action;
        Tag = tag;
    }

    public virtual string? Tag { get; }

    public void AddAppraisal(Appraisal<T> appraisal) {
        appraisals.Add(appraisal);
    }

    protected float ScoreAppraisal(Appraisal<T> appraisal) {
        var score = appraisal.TransformedScore();

        if (Reasoner<T>.trace) lastScores[appraisal] = score;// log last score

        return score;
    }

    public abstract float Score();

    public float NormalizedScore() {
        return Score() / appraisals.Count;
    }
}