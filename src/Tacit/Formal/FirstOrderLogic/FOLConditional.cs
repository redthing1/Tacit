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
}

public record class FOLConditional(FOLIfExpression Antecedent, FOLThenExpression Consequent) : FOLRuleExpression(new FOLRuleExpression[] {
    Antecedent, Consequent
}) {
    public override string? ExpressionType => "IfThen";
}