using System.Reflection;

namespace Tacit.Legacy.Plan;

/// <summary>
///     automatically implements the more tedious Clone, Equality, and HashCode methods
///     make sure only the model data fields are properties
///     any and all properties will be included in cloning, equality, and hashcode calculation
///     however, this comes at a performance cost
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SmartActionPlanningModel<T> : ActionPlanningModel<T> where T : new() {
    private readonly PropertyInfo[] _propertyCache;

    public SmartActionPlanningModel() {
        var properties = GetType().GetProperties();
        _propertyCache = properties;
    }

    public override T Clone(T b) {
        for (var i = 0; i < _propertyCache.Length; i++) {
            var mine = _propertyCache[i].GetValue(this);
            _propertyCache[i].SetValue(b, mine);
        }

        return b;
    }

    public override bool Equals(T b) {
        for (var i = 0; i < _propertyCache.Length; i++) {
            var mine = _propertyCache[i].GetValue(this);
            var other = _propertyCache[i].GetValue(b);
            if (!mine.Equals(other)) return false;
        }

        return true;
    }

    public override int GetHashCode() {
        unchecked// Overflow is fine, just wrap
        {
            var hash = 17;
            for (var i = 0; i < _propertyCache.Length; i++) {
                hash = hash * 23 + _propertyCache[i].GetValue(this).GetHashCode();
            }

            return hash;
        }
    }
}