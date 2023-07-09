using System;
using System.Collections.Generic;
using Tacit.Primer;

namespace Tacit.Framework.Utility;

public abstract class Appraisal<T> {

    /// <summary>
    ///     a utility to attach a "postprocess/transform" function to the score
    /// </summary>
    private readonly List<Func<float, float>> _transforms = new();

    protected T context;

    public Appraisal(T context) {
        this.context = context;
    }

    public abstract float Score();

    public float TransformedScore() {
        var val = Score();
        if (_transforms.Count > 0) {
            var v = val;
            for (var i = 0; i < _transforms.Count; i++) {
                v = _transforms[i].Invoke(v);
            }

            val = v;
        }

        return val;
    }

    public Appraisal<T> Negate() {
        _transforms.Add(v => -v);
        return this;
    }

    public Appraisal<T> Inverse() {
        _transforms.Add(v => 1f - v);
        return this;
    }

    public Appraisal<T> Clamp(float limit) {
        _transforms.Add(v => Mathf.Clamp(v, min: 0, limit));
        return this;
    }

    public Appraisal<T> Scale(float scale) {
        _transforms.Add(v => scale * v);
        return this;
    }
}