namespace Tacit.Formal.FirstOrderLogic;

public static class FOLMatcher {

    public static bool TrySubstitute(FOLFact fact, FOLRule rule, FOLMatchContext context) {
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
            }
        }

        // at this point, we have successfully bound all variables
        return true;
    }
}