using System.Collections.Generic;
using System.Linq;

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

    public override FOLRuleExpression Duplicate() {
        return new FOLOrExpression(Children.Select(c => c.Duplicate()).ToArray());
    }
}