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
        var populatedFacts = Expression.PopulateSingle(context);
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

public record class FOLDeleteExpression(FOLRuleExpression Expression) : FOLRuleExpression(new[] {
    Expression
}) {
    public override string? ExpressionType => "Delete";

    public List<FOLFact> Delete(FOLKnowledgeBase kb, FOLMatchContext context) {
        var populatedFacts = Expression.PopulateSingle(context);
        var deletedFacts = new List<FOLFact>();
        foreach (var fact in populatedFacts) {
            // ensure the fact is known
            if (kb.Ask(fact)) {
                deletedFacts.Add(fact);
            }
        }
        if (deletedFacts.Any()) {
            kb.Remove(deletedFacts);
        }
        return deletedFacts;
    }
}

public record class FOLConditional : FOLRuleExpression {
    public override string? ExpressionType => "IfThen";
    public FOLIfExpression Antecedent { get; init; }
    public FOLThenExpression? Consequent { get; init; }
    public FOLDeleteExpression? Deleter { get; init; }


    public FOLConditional(FOLIfExpression antecedent, FOLThenExpression consequent) : base(antecedent, consequent) {
        Antecedent = antecedent;
        Consequent = consequent;
    }

    public FOLConditional(FOLIfExpression antecedent, FOLDeleteExpression deleter) : base(antecedent, deleter) {
        Antecedent = antecedent;
        Deleter = deleter;
    }

    public FOLConditional(FOLIfExpression antecedent, FOLThenExpression consequent, FOLDeleteExpression deleter) : base(antecedent, consequent, deleter) {
        Antecedent = antecedent;
        Consequent = consequent;
        Deleter = deleter;
    }

    public override bool Apply(FOLKnowledgeBase kb) {
        // get all matches for the antecedent
        var matchBindings = Antecedent.MatchAllPossible(kb, null);
        if (matchBindings.Count == 0) {
            // no matches
            return false;
        }

        var numProductions = 0;
        var numDeletions = 0;

        if (Consequent is not null) {
            // produce new facts from consequent
            foreach (var context in matchBindings) {
                var producedFacts = Consequent.Produce(kb, context);
                numProductions += producedFacts.Count;
            }
        }

        if (Deleter is not null) {
            // delete facts from delete clause
            foreach (var context in matchBindings) {
                var deletedFacts = Deleter.Delete(kb, context);
                numDeletions += deletedFacts.Count;
            }
        }
        
        // productions can cause more productions
        // however, deletions cannot cause more deletions

        return numProductions > 0;
    }
}