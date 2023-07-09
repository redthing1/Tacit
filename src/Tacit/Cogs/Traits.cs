namespace Tacit.Cogs;

public abstract class Traits<TPersonality> where TPersonality : Personality {
    public abstract void Calculate(TPersonality ply);
}