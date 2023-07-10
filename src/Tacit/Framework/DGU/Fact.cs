namespace Tacit.Framework.DGU;

public interface IFact {
    string SubjectId { get; }
}

public record Fact<T>(ISmartObject Subject, string Attribute, T value, long time) : IFact {
    public string SubjectId => Subject.Id;

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

public record FactChange(string SubjectId, string Attribute, FactChangeType Change);