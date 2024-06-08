using System.Text.RegularExpressions;

namespace Tacit.Formal.FirstOrderLogic;

public class FOLParser {

    public FOLFact ParseFact(string factStr) {
        // expect syntax: predicate(referent1, referent2, ...)
        var parseRegex = new Regex(@"^(\w+)\(([\w, ]+)\)$");
        var match = parseRegex.Match(factStr);
        if (!match.Success) {
            throw new System.Exception($"invalid fact syntax: {factStr}");
        }
        var predicate = match.Groups[1].Value;
        var referents = match.Groups[2].Value.Split(", ");
        return new FOLFact(predicate, referents);
    }
}