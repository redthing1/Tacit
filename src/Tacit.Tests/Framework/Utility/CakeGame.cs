namespace Tacit.Tests.Framework.Utility;

public class CakeGame {
    // - consts
    public const int FLOUR_PER_CAKE = 100;
    public const float ENERGY_PER_CAKE = 0.05f;
    public const int ANGERY_CUSTOMERS = 10;// the number of orders where they get mad
    public const int CAKES_PER_SESSION = 4;

    public const int STORE_FLOUR = 1000;

    // - actors
    public Baker baker;

    // - state
    public int cakesBaked;
    public float fatigue;// fatigue [0,1]
    public int flour;
    public int orders;

    public CakeGame() {
        baker = new Baker(this);
    }

    public void SleepBed() {
        fatigue = 0;
    }

    public void BakeCake() {
        for (var i = 0;
             i < CAKES_PER_SESSION
             && flour > FLOUR_PER_CAKE// we have flour
             && orders > 0// we have an order
             && fatigue < 1 - ENERGY_PER_CAKE;// we shouldn't collapse while baking
             i++) {
            // bake
            flour -= FLOUR_PER_CAKE;// used flour
            cakesBaked++;// got a cake
            orders--;// completed an order 
            fatigue += ENERGY_PER_CAKE;// used some energy
        }
    }

    public void BuyFlour() {
        flour += STORE_FLOUR;
    }

    /// <summary>
    ///     step the game
    /// </summary>
    /// <returns>whether the baker is alive</returns>
    public bool Step() {
        // daily orders
        orders += 2;

        // make your move, baker
        var log = baker.Act();

        // check conditions
        if (fatigue >= 1f) return false;// died of exhaustion
        if (orders >= ANGERY_CUSTOMERS) return false;// didn't do orders
        if (flour <= 0) return false;// ran out of flour

        // still alive... for now
        return true;
    }

    /// <summary>
    ///     run the game for n steps
    /// </summary>
    /// <param name="iterations"></param>
    /// <returns>whether the baker is still alive</returns>
    public bool Run(int iterations) {
        for (var i = 0; i < iterations; i++) {
            var result = Step();
            // the baker is dead
            if (!result) return false;
        }

        return true;
    }
}