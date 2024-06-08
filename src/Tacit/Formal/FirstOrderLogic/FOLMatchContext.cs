using System.Collections.Generic;

namespace Tacit.Formal.FirstOrderLogic;

public class FOLMatchContext {
    private Dictionary<string, string> _bindings = new();
    
    public FOLMatchContext() {
    }
    
    public FOLMatchContext(FOLMatchContext other) {
        _bindings = new Dictionary<string, string>(other._bindings);
    }

    public bool Bind(string variable, string value) {
        if (_bindings.TryGetValue(variable, out var existingValue)) {
            // if existing value doesn't match, fail
            return existingValue == value;
        } else {
            _bindings.Add(variable, value);
            return true;
        }
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

    public bool Has(string referent) {
        return _bindings.ContainsKey(referent);
    }

    public void UpdateFrom(FOLMatchContext other) {
        foreach (var binding in other._bindings) {
            _bindings[binding.Key] = binding.Value;
        }
    }
}