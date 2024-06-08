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

    public bool Produce(FOLKnowledgeBase kb, FOLMatchContext bindings) {
        var producedFacts = false;
        // produce new facts
        var newFacts = Expression.Populate(kb, bindings);
        kb.Add(newFacts);
        producedFacts = newFacts.Count > 0;
        return producedFacts;
    }
}

public record class FOLConditional(FOLIfExpression Antecedent, FOLThenExpression Consequent) : FOLRuleExpression(new FOLRuleExpression[] {
    Antecedent, Consequent
}) {
    public override string? ExpressionType => "IfThen";

    public override bool Matches(FOLKnowledgeBase kb, FOLMatchContext context) {
        // match the antecedent
        if (!Antecedent.Matches(kb, context)) {
            return false;
        }
        
        return true;
    }

    public override bool Apply(FOLKnowledgeBase kb) {
        // check if it matches
        var context = new FOLMatchContext();
        if (!Matches(kb, context)) {
            return false;
        }
        // produce new facts
        return Consequent.Produce(kb, context);
    }
}