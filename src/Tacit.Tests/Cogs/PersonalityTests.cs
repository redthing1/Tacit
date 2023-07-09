using Tacit.Calc;
using Xunit;

namespace Tacit.Tests.Cogs;

public class PersonalityTests {
    private readonly WriterSoul _soul;

    public PersonalityTests() {
        _soul = new WriterSoul();
    }

    [Fact]
    public void CanMapValues() {
        var vi = 0f;
        var v01 = PerMath.Map01(vi);
        Assert.Equal(expected: 0.5f, v01, precision: 4);
        var v11 = PerMath.Map11(v01);
        Assert.Equal(expected: 0f, v11, precision: 4);
    }

    [Fact]
    public void CanCreateSoul() {
        Assert.NotNull(_soul.ply);
        Assert.NotNull(_soul.emotions);
        Assert.NotNull(_soul.traits);
    }

    [Fact]
    public void CanComputeTraits() {
        // create a fixed personality
        _soul.ply = new WriterSoul.Personality(e: 0.2f, c: 0.8f);
        _soul.Recalculate();
        Assert.Equal(expected: -0.06, _soul.traits.productivity, precision: 4);
    }
}