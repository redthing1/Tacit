using System;
using System.Collections.Generic;
using Tacit.Utils;

namespace Tacit.Framework.DGU;

public interface IReadOnlyFactMemory {
    IFact? GetFactFromChangeKey(FactChange change);
    IFact? GetFact(string subjectId, string attribute);
    IFact ExpectFact(string subjectId, string attribute);
    Fact<T> ExpectFact<T>(string subjectId, string attribute);
}

public class FactMemory : IForkable<FactMemory>, IReadOnlyFactMemory {
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

    public void AddFact(IFact fact) {
        _facts.Add(fact);
        if (!_factsAtTime.ContainsKey(fact.Time)) {
            _factsAtTime[fact.Time] = new List<IFact>();
        }

        _factsAtTime[fact.Time].Add(fact);
    }

    public void UpdateFact(IFact fact) {
        // remove old fact
        RemoveFact(fact);
        // add new fact
        AddFact(fact);
    }

    private void RemoveFact(IFact fact) {
        _facts.Remove(fact);

        if (_factsAtTime.TryGetValue(fact.Time, out var factsAtTimeList)) {
            factsAtTimeList.Remove(fact);
        }
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

    public IFact? GetFact(string subjectId, string attribute) {
        var matchingFact = _facts.FindLast(f => f.SubjectId == subjectId && f.Attribute == attribute);
        if (matchingFact == null) {
            return null;
        }

        return matchingFact;
    }

    public IFact ExpectFact(string subjectId, string attribute) {
        var matchingFact = _facts.FindLast(f => f.SubjectId == subjectId && f.Attribute == attribute);
        if (matchingFact == null) {
            throw new Exception($"Fact not found: {subjectId}::{attribute}");
        }

        return matchingFact;
    }

    public Fact<T> ExpectFact<T>(string subjectId, string attribute) {
        return (Fact<T>)ExpectFact(subjectId, attribute);
    }
}