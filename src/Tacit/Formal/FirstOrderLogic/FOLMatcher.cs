using System.Collections.Generic;

namespace Tacit.Formal.FirstOrderLogic;

public static class FOLMatcher {

    public static bool MatchTrySubstitute(FOLFact fact, FOLRule rule, FOLMatchContext context) {
        // given a rule with referents of the form (?x) and a fact with referents of the form (value)
        // see if we can find variables that can be substituted to satisfy the rule

        // first, ensure they have the same number of referents
        if (fact.Referents.Length != rule.Referents.Length) {
            return false;
        }

        // then, check if the predicates match
        if (fact.Predicate != rule.Predicate) {
            return false;
        }

        // now, try to bind the variables to the values
        for (var i = 0; i < fact.Referents.Length; i++) {
            var factRft = fact.Referents[i];
            var ruleRft = rule.Referents[i];
            // TODO: using spans might make this a bit faster

            if (ruleRft.StartsWith("?")) {
                // we have a variable in the rule
                if (!context.Bind(ruleRft, factRft)) {
                    // we failed to bind the variable
                    return false;
                }
            } else {
                // we have a constant in the rule
                if (factRft != ruleRft) {
                    // the constants don't match
                    return false;
                }
                continue;
            }
        }

        // at this point, we have successfully bound all variables
        return true;
    }

    public static bool SubstituteToFact(FOLRule rule, FOLMatchContext bindings, out FOLFact? fact) {
        // given a rule and a set of bindings, substitute the bindings into the rule to produce a fact
        var referents = new List<string>();
        foreach (var referent in rule.Referents) {
            if (referent.StartsWith("?")) {
                // we have a variable
                if (bindings.Has(referent)) {
                    var value = bindings.Get(referent);
                    referents.Add(value);
                } else {
                    // we failed to find a binding
                    fact = null;
                    return false;
                }
            } else {
                // we have a constant
                referents.Add(referent);
            }
        }

        fact = new FOLFact(rule.Predicate, referents.ToArray());
        return true;
    }
    
    public static FOLRule SubstituteToSpecializedRule(FOLRule rule, FOLMatchContext bindings) {
        // given a rule and a set of bindings, substitute the bindings into the rule to produce a specialized rule
        var referents = new List<string>();
        foreach (var referent in rule.Referents) {
            if (referent.StartsWith("?")) {
                // we have a variable
                if (bindings.Has(referent)) {
                    // specialize the variable using the bound value
                    var value = bindings.Get(referent);
                    referents.Add(value);
                } else {
                    // add the variable back in
                    referents.Add(referent);
                }
            } else {
                // we have a constant
                referents.Add(referent);
            }
        }

        var specializedRule = new FOLRule(rule.Predicate, referents.ToArray());
        
        return specializedRule;
    }
}