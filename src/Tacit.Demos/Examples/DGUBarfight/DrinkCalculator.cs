namespace Tacit.Demos.Examples.DGUBarfight;

public static class DrinkCalculator {
    public const float STANDARD_DRINK_BAC = 0.02f; // BAC from a standard drink
    public const float STANDARD_DRINK_ETHANOL_ML = 14; // a standard drink is 14mL of ethanol

    /// <summary>
    /// calculate the BAC impact of a drink given its volume(mL) and ABV(%)
    /// </summary>
    /// <param name="drinkVolume"></param>
    /// <param name="drinkAbv"></param>
    /// <returns></returns>
    public static float CalculateDrinkBAC(float drinkVolume, float drinkAbv) {
        // calculate the volume of ethanol in the drink
        var ethanolVolume = drinkVolume * (drinkAbv / 100);
        // calculate the number of standard drinks in the drink
        var standardDrinks = ethanolVolume / STANDARD_DRINK_ETHANOL_ML;
        // calculate the BAC impact of the drink
        var drinkBAC = standardDrinks * STANDARD_DRINK_BAC;
        return drinkBAC;
    }
}