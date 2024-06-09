using System.Collections.Generic;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLRule(string Predicate, string[] Referents) {
    // construct rule from fact (will be constants)
    public FOLRule(FOLFact fact) : this(fact.Predicate, fact.Referents) {}

    public override string ToString() {
        return $"{Predicate}({string.Join(", ", Referents)})";
    }

    // public bool MatchOne(FOLKnowledgeBase kb, FOLMatchContext context) {
    //     // check if the rule matches the knowledge base
    //     foreach (var fact in kb.Facts) {
    //         var thisRuleBindings = new FOLMatchContext(context);
    //         bool substituteResult = FOLMatcher.MatchTrySubstitute(fact, this, thisRuleBindings);
    //         if (substituteResult) {
    //             // this rule matched, copy context
    //             context.UpdateFrom(thisRuleBindings);
    //             return true;
    //         }
    //     }
    //
    //     return false;
    // }

    public List<FOLMatchContext> MatchAllPossible(FOLKnowledgeBase kb, FOLMatchContext? currentContext = null) {
        var bindings = new List<FOLMatchContext>();
        foreach (var fact in kb.Facts) {
            var ruleContext = new FOLMatchContext();
            bool substituteResult = FOLMatcher.MatchTrySubstitute(fact, this, ruleContext);
            if (substituteResult) {
                bindings.Add(ruleContext);
            }
        }

        return bindings;
    }

    public FOLFact? Populate(FOLMatchContext bindings) {
        // we expect to produce 0 or 1 facts from this rule (using it as a template)
        if (FOLMatcher.SubstituteToFact(this, bindings, out var outFact)) {
            if (outFact == null) return null;
            return outFact;
        }

        return null;
    }
}