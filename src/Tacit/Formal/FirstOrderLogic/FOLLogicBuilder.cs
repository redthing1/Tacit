using System.Collections.Generic;
using System.Linq;

namespace Tacit.Formal.FirstOrderLogic;

public class FOLLogicBuilder {
    public List<FOLFact> Facts(params FOLFact[] facts) {
        return facts.ToList();
    }

    public FOLFact Fact(string predicate, params string[] arguments) {
        return new FOLFact(predicate, arguments);
    }
    
    public List<FOLRuleExpression> Rules(params FOLRuleExpression[] rules) {
        return rules.ToList();
    }

    public FOLRule Rule(string predicate, params string[] arguments) {
        return new FOLRule(predicate, arguments);
    }
    
    public FOLAndExpression And(params FOLRuleExpression[] exprs) {
        return new FOLAndExpression(exprs);
    }
    
    public FOLOrExpression Or(params FOLRuleExpression[] exprs) {
        return new FOLOrExpression(exprs);
    }
    
    public FOLNotExpression Not(FOLRuleExpression expr) {
        return new FOLNotExpression(expr);
    }
    
    public FOLIfExpression If(FOLRuleExpression antecedent) {
        return new FOLIfExpression(antecedent);
    }
    
    public FOLThenExpression Then(FOLRuleExpression consequent) {
        return new FOLThenExpression(consequent);
    }
    
    public FOLDeleteExpression Delete(FOLRuleExpression expr) {
        return new FOLDeleteExpression(expr);
    }
    
    public FOLConditional Cond(FOLIfExpression ifExpr, FOLThenExpression thenExpr) {
        return new FOLConditional(ifExpr, thenExpr);
    }
}