using Xunit;

namespace Tacit.Tests.Framework.Utility;

public class UtilityTests {
    public CakeGame game;

    public UtilityTests() {
        game = new CakeGame();
    }

    public Baker Baker => game.baker;

    [Fact]
    public void CanExecuteReasoner() {
        var results = Baker.Think();
        Assert.NotEmpty(results);
    }

    [Fact]
    public void CanWinCakeGame() {
        Assert.True(game.cakesBaked == 0);
        var win = game.Run(50);
        Assert.True(win);
        Assert.True(game.cakesBaked > 0);
    }

    [Fact]
    public void CanWinCakeGameManyIterations() {
        Assert.True(game.cakesBaked == 0);
        var win = game.Run(10000);
        Assert.True(win);
        Assert.True(game.cakesBaked > 0);
    }
}