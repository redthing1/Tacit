using BenchmarkDotNet.Attributes;
using Tacit.Tests.Framework.Utility;
using Xunit;

namespace Tacit.Perf.Framework;

[MemoryDiagnoser]
public class CakeGamePerf {
    private CakeGame _game;
    [Params(100, 1000, 10000, 100000)] public int n;

    [GlobalSetup]
    public void Setup() {
        _game = new CakeGame();
    }

    [Benchmark]
    public bool RunCakeGame() {
        var result = _game.Run(n);
        Assert.True(result);
        return result;
    }
}