using Tacit.Framework.DGU;
using Xunit;

namespace Tacit.Tests.Framework.DGU;

public class DguBasicTests {
    [Fact]
    public void CanInitAgent() {
        var agent = new DguAgent();
    }

    [Fact]
    public void CanRunEmptySta() {
        var agent = new DguAgent();

        agent.Sense();
        agent.Think();
        agent.Act();
    }
}