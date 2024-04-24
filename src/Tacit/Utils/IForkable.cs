namespace Tacit.Utils; 

public interface IForkable<T> {
    T Fork();
}