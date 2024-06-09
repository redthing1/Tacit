using System.Collections.Generic;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLOrExpression(FOLRuleExpression[] Children) : FOLRuleExpression(Children) {
    public override string? ExpressionType => "Or";

    public override List<FOLMatchContext> MatchAllPossible(FOLKnowledgeBase kb, FOLMatchContext? currentContext = null) {
        var satisfactoryBindings = new List<FOLMatchContext>();
        foreach (var condition in Children) {
            // get all bindings that can satisfy this condition
            var bindings = condition.MatchAllPossible(kb, currentContext);
            satisfactoryBindings.AddRange(bindings);
        }
        
        return satisfactoryBindings;
    }
}