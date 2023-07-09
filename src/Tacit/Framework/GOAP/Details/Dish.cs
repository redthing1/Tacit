// Containers for reusable planning states
namespace Tacit.Framework.GOAP.Details; 

internal abstract class Dish<T> where T : class {

    protected T proto, state;

    public abstract void Init(T prototype);
    public abstract T Avail();

    public virtual void Invalidate() {}
    public void Consume() {
        state = null;
    }

    public static Dish<T> Create(T s, bool safe) {
        return s is Clonable<T> && !safe
            ? new DirtyDish<T>()
            : new PolyDish<T>();
    }
}

// Dirty dish assumes failing actions do not mutate model state
// Faster because no 'cleaning the dish' after a failed action.
internal class DirtyDish<T> : Dish<T> where T : class {
    private Clonable<T> clonable;

    private bool dirty;

    public override void Init(T prototype) {
        clonable = (Clonable<T>)prototype;
        proto = prototype;
        dirty = true;
    }

    public override T Avail() {
        if (dirty) state = Clone();
        dirty = false;
        return state;
    }

    public override void Invalidate() {
        dirty = true;
    }

    private T Clone() {
        return clonable.Clone(state ?? clonable.Allocate());
    }
}

// Cleans after every action; supports serial clones
internal class PolyDish<T> : Dish<T> where T : class {

    public override void Init(T prototype) { proto = prototype; }

    public override T Avail() {
        state = Clone();
        return state;
    }

    private T Clone() {
        return proto is Clonable<T> src
            ? src.Clone(state ?? src.Allocate())
            : CloneUtil.DeepClone(proto);
    }
}