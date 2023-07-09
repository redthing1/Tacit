using System;

namespace Tacit.Cogs;

public abstract class Emotions {
    public abstract float[] Vec { get; }
    public virtual float Falloff { get; } = 0.9f;

    /// <summary>
    ///     update the emotions. they will fade with time, exponentially tending toward zero
    /// </summary>
    public void Tick() {
        for (var i = 0; i < Vec.Length; i++) {
            Vec[i] = Falloff * Vec[i];
        }
    }

    /// <summary>
    ///     resets all emotions to neutral values
    /// </summary>
    public void Reset() {
        for (var i = 0; i < Vec.Length; i++) {
            Vec[i] = 0f;
        }
    }

    /// <summary>
    ///     spikes an emotion to a given level if more intense than current emotion.
    ///     does nothing if the spike is less intense than the current emotion.
    /// </summary>
    /// <param name="emotion"></param>
    /// <param name="val"></param>
    protected void Spike(ref float emotion, float val) {
        if (Math.Abs(val) > Math.Abs(emotion)) emotion = val;
    }
}