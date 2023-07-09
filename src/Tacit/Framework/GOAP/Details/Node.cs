using System;

namespace Tacit.Framework.GOAP.Details; 

public class Node<T> : Base {
    private readonly object effect;

    public readonly Node<T> prev;
    public readonly Option source;
    public readonly T state;
    public float value;

    public Node(
        Option planningAction,
        T result,
        Node<T> prev = null,
        float cost = 0f
    ) {
        source = Assert(planningAction, "Action");
        ;
        state = Assert(result, "Result");
        this.prev = prev;
        this.cost = cost + (prev?.cost ?? 0f);
    }

    // TODO - object should be System.Action and separate
    // constructor for init state if wanted
    public Node(
        object effect,
        T result,
        Node<T> prev = null,
        float cost = 0f
    ) {
        this.effect = Assert(effect, "Action");
        state = Assert(result, "Result");
        this.prev = prev;
        this.cost = cost + (prev?.cost ?? 0f);
    }
    public float cost { get; }

    public object action => effect ?? source.Method.Name;

    public static implicit operator string(Node<T> x) {
        return (string)x.Head();
    }

    public static implicit operator Delegate(Node<T> x) {
        return (Delegate)x.Head();
    }

    // Regress to first applicable action; root (init state) excl.
    public object Head() {
        return prev?.prev == null ? action : prev.Head();
    }

    public Node<T>[] Path(int n = 1) {
        Node<T>[] @out;
        if (prev == null) {
            @out = new Node<T>[n];
        } else {
            @out = prev.Path(n + 1);
        }
        @out[@out.Length - n] = this;
        return @out;
    }

    public string PathToString() {
        var path = Path();
        var s = "";
        foreach (var k in path) {
            s += k + '\n';
        }
        return s;
    }

    public override string ToString() {
        return $"[{value:0.0} :: {effect} => {state}]"
            .Replace("System.Object", "object");
    }
}