using System.Collections.Generic;
using Tacit.Framework.Utility;
using Tacit.Framework.Utility.Considerations;

namespace Tacit.Tests.Framework.Utility;

public partial class Baker {
    private readonly Dictionary<Consideration<CakeGame>, float> _reasonerResults = new();

    /// <summary>
    ///     the game state
    /// </summary>
    public CakeGame game;

    /// <summary>
    ///     the utility reasoner
    /// </summary>
    public Reasoner<CakeGame> reasoner;

    public Baker(CakeGame game) {
        this.game = game;
        BuildReasoner();
    }

    private void BuildReasoner() {
        reasoner = new Reasoner<CakeGame>();
        reasoner.scoreType = Reasoner<CakeGame>.ScoreType.Raw;

        var sleepConsid = new ThresholdSumConsideration<CakeGame>(game.SleepBed, threshold: 0.6f, "sleep");
        sleepConsid.AddAppraisal(new Sleepy(game));
        sleepConsid.AddAppraisal(new Backlogged(game).Scale(0.3f).Negate());
        reasoner.AddConsideration(sleepConsid);

        var bakeConsid = new SumConsideration<CakeGame>(game.BakeCake, "bake");
        bakeConsid.AddAppraisal(new Backlogged(game));
        reasoner.AddConsideration(bakeConsid);

        var shopConsid = new SumConsideration<CakeGame>(game.BuyFlour, "shop");
        shopConsid.AddAppraisal(new LowFlour(game));
        reasoner.AddConsideration(shopConsid);
    }

    public string Act() {
        Think();
        // choose the highest ranked option
        var chosen = reasoner.Choose(_reasonerResults);
        // execute the action
        chosen.action();
        // return the tag
        return chosen.Tag;
    }

    public Dictionary<Consideration<CakeGame>, float> Think() {
        reasoner.Execute(_reasonerResults);
        return _reasonerResults;
    }
}