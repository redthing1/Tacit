using System.Collections.Generic;
using System.Linq;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLIfExpression(FOLRuleExpression Expression) : FOLRuleExpression(new[] {
    Expression
}) {
    public override string? ExpressionType => "If";
}

public record class FOLThenExpression(FOLRuleExpression Expression) : FOLRuleExpression(new[] {
    Expression
}) {
    public override string? ExpressionType => "Then";

    public List<FOLFact> Produce(FOLKnowledgeBase kb, FOLMatchContext context) {
        var populatedFacts = Expression.Populate(kb, context);
        var newFacts = new List<FOLFact>();
        foreach (var newFact in populatedFacts) {
            // ensure the fact is not already known
            if (!kb.Ask(newFact)) {
                newFacts.Add(newFact);
            }
        }
        if (newFacts.Any()) {
            kb.Add(newFacts);
        }
        return newFacts;
    }
}

public record class FOLConditional(FOLIfExpression Antecedent, FOLThenExpression Consequent) : FOLRuleExpression(new FOLRuleExpression[] {
    Antecedent, Consequent
}) {
    public override string? ExpressionType => "IfThen";

    public override bool Apply(FOLKnowledgeBase kb) {
        // get all matches for the antecedent
        var matchBindings = Antecedent.MatchAllPossible(kb, null);
        if (matchBindings.Count == 0) {
            // no matches
            return false;
        }

        var numProductions = 0;

        foreach (var context in matchBindings) {
            var producedFacts = Consequent.Produce(kb, context);
            numProductions += producedFacts.Count;
        }

        return numProductions > 0;
    }
}