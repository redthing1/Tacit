namespace Tacit.Formal.FirstOrderLogic;

public record class FOLAndExpression(FOLRuleExpression[] Children) : FOLRuleExpression(Children) {
    public override string? ExpressionType => "And";
}