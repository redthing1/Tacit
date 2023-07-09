namespace Tacit.Cogs;

public abstract class Sentient<TPersonality, TTraits, TEmotions>
    where TPersonality : Personality, new()
    where TTraits : Traits<TPersonality>, new()
    where TEmotions : Emotions, new() {
    public TEmotions emotions;
    public TPersonality ply;
    public TTraits traits;

    // public bool calculated => ply != null;

    public Sentient() {
        ply = new TPersonality();
        traits = new TTraits();
        emotions = new TEmotions();
    }

    /// <summary>
    ///     recalculate (and reset) values using updated personality
    /// </summary>
    public void Recalculate() {
        traits.Calculate(ply);
        emotions.Reset();
    }

    public void Tick() {
        emotions.Tick();
    }

    public override string ToString() {
        return $"{GetType().Name}(ply: {ply}, traits: {traits}, emotions: {emotions})";
    }
}