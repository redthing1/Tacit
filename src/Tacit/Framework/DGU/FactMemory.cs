using System;
using System.Collections.Generic;

namespace Tacit.Framework.DGU;

public class FactMemory : IForkable<FactMemory> {
    private List<IFact> _facts = new();
    private Dictionary<long, List<IFact>> _factsAtTime = new();

    public FactMemory() {
        Initialize();
    }

    public void Initialize() {
        // reset everything
        _facts.Clear();
        _factsAtTime.Clear();
    }

    public void Add(IFact fact) {
        _facts.Add(fact);
        if (!_factsAtTime.ContainsKey(fact.Time)) {
            _factsAtTime[fact.Time] = new List<IFact>();
        }
        _factsAtTime[fact.Time].Add(fact);
    }

    public void Update(IFact fact) {
        // remove old fact
        Remove(fact);
        // add new fact
        Add(fact);
    }
    private void Remove(IFact fact) {
        _facts.Remove(fact);
        _factsAtTime[fact.Time].Remove(fact);
    }

    public List<IFact> GetFactsAtTime(long time) {
        if (!_factsAtTime.ContainsKey(time)) {
            return new List<IFact>();
        }
        return _factsAtTime[time];
    }

    /// <summary>
    /// fork this fact memory into an independent instance but containing the same facts
    /// </summary>
    /// <returns></returns>
    public FactMemory Fork() {
        var clone = new FactMemory();
        clone._facts = new List<IFact>(_facts);
        clone._factsAtTime = new Dictionary<long, List<IFact>>(_factsAtTime);
        return clone;
    }
    
    public IFact? GetFactFromChangeKey(FactChange change) {
        // find the most recent fact about the same subject, on the same attribute
        var matchingFact = _facts.FindLast(f => f.SubjectId == change.SubjectId && f.Attribute == change.Attribute);
        if (matchingFact == null) {
            return null;
        }
        return matchingFact;
    }
}