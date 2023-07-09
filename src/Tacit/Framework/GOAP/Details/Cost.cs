namespace Tacit.Framework.GOAP.Details; 

// Note: in general cost values not strictly positive are unsafe;
// however this isn't checked here since it depends on the solver
public readonly struct Cost {

    public readonly bool done;
    public readonly float cost;

    private Cost(bool flag, float c) {
        done = flag;
        cost = c;
    }

    public static implicit operator Cost(bool flag) {
        return new Cost(flag, c: 1);
    }

    public static implicit operator Cost(int cost) {
        return new Cost(flag: true, cost);
    }

    public static implicit operator Cost(float cost) {
        return new Cost(flag: true, cost);
    }

    public static implicit operator Cost((object, float cost) t) {
        return new Cost(flag: true, t.cost);
    }

}