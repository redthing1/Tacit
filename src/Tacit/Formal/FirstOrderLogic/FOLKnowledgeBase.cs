using System.Collections.Generic;
using System.Linq;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLKnowledgeBase {
    public List<FOLFact> Facts { get; init; }

    public FOLKnowledgeBase(List<FOLFact> facts) {
        Facts = facts.ToList();
    }

    public bool Ask(FOLFact question) {
        // check if the fact is in the knowledge base
        foreach (var fact in Facts) {
            if (fact.IsEqual(question)) {
                return true;
            }
        }

        return false;
    }

    public void Add(FOLFact newFact) {
        Facts.Add(newFact);
    }

    public void Remove(FOLFact deleteFact) {
        for (var i = 0; i < Facts.Count; i++) {
            if (Facts[i].IsEqual(deleteFact)) {
                Facts.RemoveAt(i);
                return;
            }
        }
    }

    public void Add(List<FOLFact> newFacts) {
        foreach (var fact in newFacts) {
            Add(fact);
        }
    }

    public void Remove(List<FOLFact> deleteFacts) {
        foreach (var fact in deleteFacts) {
            Remove(fact);
        }
    }
}