using System.Collections.Generic;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLNotExpression(FOLRuleExpression Expression) : FOLRuleExpression(new[] {
    Expression
}) {
    public override string? ExpressionType => "Not";

    public override List<FOLMatchContext> MatchAllPossible(FOLKnowledgeBase kb, FOLMatchContext? currentContext = null) {
        if (currentContext == null) currentContext = new FOLMatchContext();
        
        var populatedFacts = Expression.PopulateSingle(currentContext);
        FOLRuleExpression newKey;
        if (populatedFacts.Count > 0) {
            newKey = new FOLRule(populatedFacts[0]);
        } else {
            newKey = Expression;
        }

        var bindings = newKey.MatchAllPossible(kb, null);
        var matched = bindings.Count > 0;

        if (matched) {
            // the negated fact matched, so no contexts satisfy this expression
            return new List<FOLMatchContext>();
        } else {
            // the negated fact was not matched, so the context satisfies this expression
            return new List<FOLMatchContext> {
                currentContext
            };
        }
    }
}