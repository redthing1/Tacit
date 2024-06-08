namespace Tacit.Formal.FirstOrderLogic;

public record class FOLRule(string Predicate, string[] Referents) {
    public override string ToString() {
        return $"{Predicate}({string.Join(", ", Referents)})";
    }

    public bool MatchOne(FOLKnowledgeBase kb, FOLMatchContext context) {
        // check if the rule matches the knowledge base
        foreach (var fact in kb.Facts) {
            context.Clear();
            bool substituteResult = FOLMatcher.TrySubstitute(fact, this, context);
            if (substituteResult) {
                // this rule matched
                return true;
            }
        }

        return false;
    }
}