using System;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLAndExpression(FOLRuleExpression[] Children) : FOLRuleExpression(Children) {
    public override string? ExpressionType => "And";

    public override bool Matches(FOLKnowledgeBase kb, FOLMatchContext context) {
        // for and to match, all children must match with the same context
        var andContext = new FOLMatchContext(context);
        
        foreach (var child in Children) {
            if (!child.Matches(kb, andContext)) {
                return false;
            }
        }
        
        context.UpdateFrom(andContext);

        return true;
    }
}