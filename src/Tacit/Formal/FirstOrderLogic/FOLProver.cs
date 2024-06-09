using System;
using System.Collections.Generic;

namespace Tacit.Formal.FirstOrderLogic;

public class FOLProver {
    /// <summary>
    /// apply forward chaining to the knowledge base
    /// </summary>
    /// <param name="rules"></param>
    /// <param name="kb"></param>
    /// <param name="maxSteps"></param>
    public void ForwardChain(List<FOLRuleExpression> rules, FOLKnowledgeBase kb, long maxSteps = 1000) {
        // apply rules to a dataset in order

        // whether new facts were produced
        bool newFactsProduced = true;

        // apply rules until no new facts are produced
        for (var i = 0; i < maxSteps; i++) {
            newFactsProduced = false;
            foreach (var rule in rules) {
                bool produced = rule.Apply(kb);
                if (produced) {
                    newFactsProduced = true;
                }
            }

            if (!newFactsProduced) {
                // no new facts were produced, we are done
                break;
            }
        }
    }

    public FOLRuleExpression Simplify(FOLRuleExpression expression) {
        // simplify an expression

        return expression;
    }

    public FOLRuleExpression? BackwardChain(List<FOLRuleExpression> rules, FOLFact hypothesis) {
        List<FOLRuleExpression> waysToProve = new();

        var hypothesisKb = new FOLKnowledgeBase(new List<FOLFact> {
            hypothesis
        });

        foreach (var rule in rules) {
            // rule must be a conditional, with a consequent
            if (rule is not FOLConditional conditional) {
                throw new ArgumentException("Rule must be a conditional");
            }
            var condRule = (FOLConditional)rule;
            if (condRule.Consequent == null) {
                throw new ArgumentException("Conditional must have a consequent");
            }

            // get bindings to make the consequent match the hypothesis
            var consequentBindings = condRule.Consequent.MatchAllPossible(hypothesisKb);
            if (consequentBindings.Count == 0) {
                // no way to prove this rule
                continue;
            }

            // we expect exactly one binding
            if (consequentBindings.Count > 1) {
                throw new ArgumentException("Consequent must have exactly one binding");
            }
            var consequentBinding = consequentBindings[0];

            // substitute the binding into the antecedent
            var antecedentExpression = condRule.Antecedent.Expression.PopulateSpecialized(consequentBinding);
            antecedentExpression = Simplify(antecedentExpression);

            // ensure the antecedent expression has no variables
            if (!antecedentExpression.IsConstant()) {
                throw new ArgumentException("Antecedent must be fully instantiated");
            }
            
            // we are only capable of backchaining with depth=1 at most
            // ensure antecedent children are all simple (non-compound) rules
            if (antecedentExpression is FOLAndExpression) {
                foreach (var child in antecedentExpression.Children) {
                    // ensure child expression is simple
                    if (!child.IsSimple()) {
                        throw new ArgumentOutOfRangeException("child", child, "Antecedent child expression must be simple");
                    }
                    // ensure child is fully instantiated
                    if (!child.SingleRule!.IsConstant()) {
                        throw new ArgumentException("Antecedent child must be fully instantiated");
                    }
                }
            }

            if (antecedentExpression is FOLAndExpression) {
                // if the antecedent is a conjunction, we must prove all subclauses
                var subgoalTrees = new List<FOLRuleExpression>();
                foreach (var subgoal in antecedentExpression.Children) {
                    // convert this simple, instantiated rule to a fact
                    var subgoalFact = subgoal.SingleRule!.ToFact();
                    var subgoalTree = BackwardChain(rules, subgoalFact);
                    if (subgoalTree != null) {
                        // add to ways to prove
                        subgoalTrees.Add(subgoalTree);
                    }
                }
                // create an and expression from the subgoal trees
                var andSubgoalTree = new FOLAndExpression(subgoalTrees.ToArray());
                waysToProve.Add(andSubgoalTree);
            } else if (antecedentExpression.IsSimple()) {
                // antecedent is a single rule
                // then it's a prerequisite
                waysToProve.Add(antecedentExpression);
                // convert this simple, instantiated rule to a fact
                var antecedentFact = antecedentExpression.SingleRule!.ToFact();
                // now we must try to satisfy this subgoal
                var subgoalTree = BackwardChain(rules, antecedentFact);
                if (subgoalTree != null) {
                    // add to ways to prove
                    waysToProve.Add(subgoalTree);
                }
            }
        }

        if (waysToProve.Count == 0) {
            // no way to prove the hypothesis
            return null;
        }

        if (waysToProve.Count == 1) {
            // only one way to prove the hypothesis
            var wayToProve = waysToProve[0];
            return wayToProve;
        } else {
            // there are multiple ways to prove, create a disjunction
            var orWaysToProve = new FOLOrExpression(waysToProve.ToArray());
            return orWaysToProve;
        }

        return null;
    }
}