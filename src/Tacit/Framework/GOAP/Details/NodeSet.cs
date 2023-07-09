using System;
using System.Collections.Generic;
using static Tacit.Framework.GOAP.Details.Strings;

namespace Tacit.Framework.GOAP.Details; 

public class NodeSet<T> : Base {
    private int capacity;
    private Heuristic<T> h;
    private readonly List<Node<T>> list = new();

    private float precision;
    internal bool sorted;
    private readonly HashSet<T> states = new();

    public bool capacityExceeded => count > capacity;
    public int visited => states.Count;
    public int count => list.Count;

    public NodeSet<T> Init(
        T x,
        Heuristic<T> h,
        bool sorted = true,
        int capacity = 128,
        float precision = 0
    ) {
        if (visited > 0 || count > 0) {
            throw new Exception("Clear NodeSet first");
        }
        this.h = h;
        this.sorted = sorted;
        this.capacity = capacity;
        this.precision = precision;
        states.Add(Assert(x, "Initial state"));
        list.Add(new Node<T>(INITIAL_STATE, x));
        return this;
    }

    public static implicit operator bool(NodeSet<T> self) {
        return self.count > 0 && self.count <= self.capacity;
    }

    public bool Insert(Node<T> n) {
        if (!states.Add(n.state)) return false;
        if (sorted) {
            n.value = n.cost + (h != null ? h(n.state) : 0);
            if (precision > 0) n.value = (int)(n.value / precision);
            for (var i = list.Count - 1; i >= 0; i--) {
                if (n.value <= list[i].value) {
                    list.Insert(i + 1, n);
                    return true;
                }
            }
        }
        list.Insert(index: 0, n);
        return true;
    }

    public Node<T> Pop() {
        var i = list.Count - 1;
        var n = list[i];
        list.RemoveAt(i);
        return n;
    }

    public void Clear() {
        states.Clear();
        list.Clear();
    }
}