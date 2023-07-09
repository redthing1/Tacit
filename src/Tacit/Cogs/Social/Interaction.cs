namespace Tacit.Cogs.Social;

public abstract class Interaction<TContext> {
    public abstract void Run(params TContext[] participants);
}