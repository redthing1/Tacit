namespace Tacit.Framework.GOAP.Details; 

public interface ActionHandler {

    void Effect<T>(object action, GameAI<T> client) where T : class;
}