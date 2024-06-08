namespace Tacit.Formal.FirstOrderLogic;

public readonly record struct FOLFact(string Predicate, string[] Referents) {
    public bool IsEqual(FOLFact other) {
        if (Predicate != other.Predicate) return false;
        if (Referents.Length != other.Referents.Length) return false;
        for (var i = 0; i < Referents.Length; i++) {
            if (Referents[i] != other.Referents[i]) return false;
        }
        return true;
    }
}