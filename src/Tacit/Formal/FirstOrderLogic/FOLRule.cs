using System.Collections.Generic;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLRule(string Predicate, string[] Referents) {
    public override string ToString() {
        return $"{Predicate}({string.Join(", ", Referents)})";
    }

    public bool MatchOne(FOLKnowledgeBase kb, FOLMatchContext context) {
        // check if the rule matches the knowledge base
        foreach (var fact in kb.Facts) {
            var thisRuleBindings = new FOLMatchContext(context);
            bool substituteResult = FOLMatcher.MatchTrySubstitute(fact, this, thisRuleBindings);
            if (substituteResult) {
                // this rule matched, copy context
                context.UpdateFrom(thisRuleBindings);
                return true;
            }
        }

        return false;
    }

    public FOLFact? Populate(FOLKnowledgeBase kb, FOLMatchContext bindings) {
        // we expect to produce 0 or 1 facts from this rule (using it as a template)
        if (FOLMatcher.SubstituteFromBindings(this, bindings, out var newFact)) {
            if (newFact == null) return null;
            // check if the fact is already known
            if (kb.Ask(newFact.Value)) {
                return null;
            }
            
            return newFact;
        }
        
        return null;
    }
}