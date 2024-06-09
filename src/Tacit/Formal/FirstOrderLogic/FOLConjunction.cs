using System;
using System.Collections.Generic;
using System.Linq;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLAndExpression(FOLRuleExpression[] Children) : FOLRuleExpression(Children) {
    public override string? ExpressionType => "And";

    public override List<FOLMatchContext> MatchAllPossible(FOLKnowledgeBase kb, FOLMatchContext? currentContext = null) {
        // for and to match, all children must match with the same context
        var matches = MatchAllPossibleIter(kb, Children, null).ToList();
        return matches;
    }

    public IEnumerable<FOLMatchContext> MatchAllPossibleIter(FOLKnowledgeBase kb, IEnumerable<FOLRuleExpression> conditions, FOLMatchContext? cumulativeContext) {
        if (cumulativeContext == null) {
            cumulativeContext = new FOLMatchContext();
        }

        var conditionList = conditions.ToList();
        if (conditionList.Count == 0) {
            yield return cumulativeContext;
            yield break;
        }

        var condition = conditionList.First();
        var allBindings = condition.MatchAllPossible(kb, cumulativeContext);
        foreach (var binding in allBindings) {
            // try to update this match's binding from the cumulative context, to ensure they are compatible
            var thisMatchContext = new FOLMatchContext(binding);
            if (thisMatchContext.IsCompatibleWith(cumulativeContext)) {
                // compatible
                thisMatchContext.UpdateFrom(cumulativeContext);
                // try to match the rest of the conditions
                var bindings2 = MatchAllPossibleIter(kb, conditionList.Skip(1), thisMatchContext).ToList();
                if (bindings2.Count == 0) {
                    // no match, skip
                    continue;
                }
                foreach (var binding2 in bindings2) {
                    yield return binding2;
                }
            } else {
                // incompatible, skip
                continue;
            }
        }
    }
}