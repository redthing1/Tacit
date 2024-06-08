using System.Collections.Generic;
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

    public List<FOLFact> ParseFacts(string factListStr) {
        // semicolon separated list of facts
        // replace all whitespace with a single space
        factListStr = Regex.Replace(factListStr, @"\s+", " ");
        var factStrs = factListStr.Split("; ");
        var facts = new List<FOLFact>();
        foreach (var factStr in factStrs) {
            facts.Add(ParseFact(factStr));
        }
        return facts;
    }
}