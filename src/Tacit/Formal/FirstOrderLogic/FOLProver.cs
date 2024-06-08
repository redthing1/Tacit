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
}