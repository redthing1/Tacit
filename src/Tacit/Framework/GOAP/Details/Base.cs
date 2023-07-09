using System;

namespace Tacit.Framework.GOAP.Details; 

public class Base {

    protected T Assert<T>(T arg, string name) {
        if (arg == null) {
            throw new NullReferenceException(
                $"{name} cannot be null");
        }
        return arg;
    }
}