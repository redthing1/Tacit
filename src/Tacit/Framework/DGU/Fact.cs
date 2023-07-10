namespace Tacit.Framework.DGU;

public interface IFact {
    string SubjectId { get; }
    string Attribute { get; }
    long Time { get; }
}

public record Fact<T>(ISmartObject Subject, string Attribute, T value, long time) : IFact {
    public string SubjectId => Subject.Id;
    public long Time => time;

    public override string ToString() {
        return $"{Subject}::{Attribute}={value}@{time}";
    }
}

public enum FactChangeType {
    Unknown,
    Increase,
    Decrease,
    MustExist,
    MustNotExist,
}

public record FactChange(string SubjectId, string Attribute, FactChangeType Change) {
    public virtual bool Equals(FactChange? other) {
        if (other is null) {
            return false;
        }
        return SubjectId == other.SubjectId && Attribute == other.Attribute && Change == other.Change;
    }
}