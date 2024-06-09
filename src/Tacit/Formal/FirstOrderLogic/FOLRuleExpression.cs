using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLRuleExpression {
    public virtual string? ExpressionType => null;
    public FOLRule? SingleRule { get; init; } = null;
    public FOLRuleExpression[] Children { get; init; }

    public FOLRuleExpression(IEnumerable<FOLRuleExpression> children) {
        Children = children.ToArray();
    }

    public FOLRuleExpression(params FOLRuleExpression[] children) {
        Children = children;
    }

    public FOLRuleExpression(FOLRule rule) {
        SingleRule = rule;
    }

    // implicit constructor for FOLRule
    public static implicit operator FOLRuleExpression(FOLRule rule) => new FOLRuleExpression(rule);

    public override string ToString() {
        var sb = new StringBuilder();
        if (SingleRule != null) {
            sb.Append(SingleRule);
        } else {
            sb.Append(ExpressionType ?? "");
            sb.Append("(");
            for (var i = 0; i < Children.Length; i++) {
                if (i > 0) sb.Append(", ");
                sb.Append(Children[i]);
            }
            sb.Append(")");
        }
        return sb.ToString();
    }

    /// <summary>
    /// apply the expression to the knowledge base, and return whether new facts were produced
    /// </summary>
    /// <param name="kb"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual bool Apply(FOLKnowledgeBase kb) {
        throw new NotImplementedException();
    }

    public virtual List<FOLMatchContext> MatchAllPossible(FOLKnowledgeBase kb, FOLMatchContext? currentContext = null) {
        if (SingleRule != null) {
            // single rule
            var bindings = SingleRule.MatchAllPossible(kb, currentContext);
            return bindings;
        } else {
            // compound rule

            if (Children.Length == 1) {
                return Children[0].MatchAllPossible(kb, currentContext);
            }

            throw new NotImplementedException();
        }
    }

    public List<FOLFact> Populate(FOLKnowledgeBase kb, FOLMatchContext bindings) {
        if (SingleRule != null) {
            // single rule
            var maybeFact = SingleRule.Populate(kb, bindings);
            var newFacts = new List<FOLFact>();
            if (maybeFact != null) {
                newFacts.Add(maybeFact.Value);
            }
            return newFacts;
        } else {
            // compound rule
            return Children.SelectMany(child => child.Populate(kb, bindings)).ToList();
        }
    }
};