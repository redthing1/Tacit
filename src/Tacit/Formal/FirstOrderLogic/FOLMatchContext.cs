using System.Collections.Generic;

namespace Tacit.Formal.FirstOrderLogic;

public class FOLMatchContext {
    private Dictionary<string, string> _bindings = new();

    public bool Bind(string variable, string value) {
        // check if already bound
        if (_bindings.ContainsKey(variable)) {
            return false;
        }

        // add binding
        _bindings.Add(variable, value);
        return true;
    }

    public bool Unbind(string variable) {
        // check if bound
        if (!_bindings.ContainsKey(variable)) {
            return false;
        }

        // remove binding
        _bindings.Remove(variable);
        return true;
    }

    public string Get(string variable) {
        return _bindings[variable];
    }
    
    public void Clear() {
        _bindings.Clear();
    }
}