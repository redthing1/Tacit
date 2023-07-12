namespace Tacit.Utils; 

public struct IdGenerator {
    private long _idCounter;

    public IdGenerator(long start = 0) {
        _idCounter = start;
    }

    public long GetNextId() => _idCounter++;
}