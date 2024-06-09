using System;
using System.Collections.Generic;

namespace Tacit.Formal.FirstOrderLogic;

public class FOLProver {
    /// <summary>
    /// apply forward chaining to the knowledge base
    /// </summary>
    /// <param name="rules"></param>
    /// <param name="kb"></param>
    public void ForwardChain(List<FOLRuleExpression> rules, FOLKnowledgeBase kb) {
        // apply rules to a dataset in order

        // whether new facts were produced
        bool newFactsProduced = true;

        // apply rules until no new facts are produced
        while (newFactsProduced) {
            newFactsProduced = false;
            foreach (var rule in rules) {
                bool produced = rule.Apply(kb);
                if (produced) {
                    newFactsProduced = true;
                }
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

            if (antecedentExpression is FOLAndExpression) {
                // if the antecedent is a conjunction, we must prove all subclauses
                throw new NotImplementedException("Conjunctions not yet supported");
            } else if (antecedentExpression.IsSimple()) {
                // antecedent is a single rule
                var antecedentRule = antecedentExpression.SingleRule!;
                // then it's a prerequisite
                waysToProve.Add(antecedentExpression);
                // ensure the rule is fully instantiated to only have constants
                if (!antecedentRule.IsConstant()) {
                    throw new ArgumentException("Antecedent must be fully instantiated");
                }
                // for now we just unwrap it
                var antecedentFact = antecedentRule.ToFact();
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

    // public FOLRuleExpression? BackchainToProve(List<FOLRuleExpression> rules, FOLKnowledgeBase kb, FOLFact hypothesis, long maxSteps) {
    //     List<FOLRuleExpression> proofRuleList = new();
    //
    //     // repeatedly backchain until we either have the required facts, or we can't prove the hypothesis
    //     for (var step = 1; step <= maxSteps; step++) {
    //         var hypothesisProof = BackwardChain(rules, hypothesis);
    //         if (hypothesisProof == null) {
    //             // impossible to prove the hypothesis
    //             return null;
    //         }
    //         proofRuleList.Add(hypothesisProof);
    //
    //         // try to execute the proof
    //         var proofKb = new FOLKnowledgeBase(kb);
    //         ForwardChain(rules, proofKb);
    //     }
    //
    //     // ran out of steps
    //     return null;
    // }
}