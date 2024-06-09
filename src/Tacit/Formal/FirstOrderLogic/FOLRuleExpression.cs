using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tacit.Formal.FirstOrderLogic;

public record class FOLRuleExpression {
    public virtual string? ExpressionType => null;
    public FOLRule? SingleRule { get; init; } = null;
    public FOLRuleExpression[] Children { get; init; }

    public bool IsSimple() {
        return SingleRule != null;
    }

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

    // clone
    public virtual FOLRuleExpression Duplicate() {
        if (SingleRule != null) {
            return new FOLRuleExpression(SingleRule);
        } else {
            throw new NotImplementedException();
        }
    }

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
        if (SingleRule != null) {
            var bindings = SingleRule.MatchAllPossible(kb);
            var newFactsProduced = false;
            foreach (var binding in bindings) {
                var newFacts = PopulateSingle(binding);
                foreach (var newFact in newFacts) {
                    // ensure the fact is not already known
                    if (!kb.Ask(newFact)) {
                        kb.Add(newFact);
                        newFactsProduced = true;
                    }
                }
            }
            return newFactsProduced;
        } else {
            throw new NotImplementedException();
        }
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

    // populate basic rules with bindings to create facts
    public List<FOLFact> PopulateSingle(FOLMatchContext bindings) {
        if (SingleRule != null) {
            // single rule
            var maybeFact = SingleRule.Populate(bindings);
            var newFacts = new List<FOLFact>();
            if (maybeFact != null) {
                newFacts.Add(maybeFact.Value);
            }
            return newFacts;
        } else {
            // invalid for compound rules
            throw new InvalidOperationException("Cannot populate compound rule");
        }
    }

    // specialize rules to substitute bindings into variables
    public FOLRuleExpression PopulateSpecialized(FOLMatchContext bindings) {
        if (SingleRule != null) {
            // single rule
            var specializedRule = FOLMatcher.SubstituteToSpecializedRule(SingleRule, bindings);

            return new FOLRuleExpression(specializedRule);
        } else {
            // compound rule
            var dupe = Duplicate();
            for (var i = 0; i < Children.Length; i++) {
                dupe.Children[i] = Children[i].PopulateSpecialized(bindings);
            }
            return dupe;
        }
    }

    public bool IsConstant() {
        if (SingleRule != null) {
            return SingleRule.IsConstant();
        } else {
            return Children.All(c => c.IsConstant());
        }
    }
};