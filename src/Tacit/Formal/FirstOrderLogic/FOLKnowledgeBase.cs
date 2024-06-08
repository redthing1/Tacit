using System.Collections.Generic;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLKnowledgeBase(List<FOLFact> Facts) {
    public bool Ask(FOLFact question) {
        // check if the fact is in the knowledge base
        foreach (var fact in Facts) {
            if (fact.IsEqual(question)) {
                return true;
            }
        }
        
        return false;
    }

    public void Add(List<FOLFact> newFacts) {
        Facts.AddRange(newFacts);
    }
}