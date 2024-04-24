namespace Tacit.Utils;

public static class WeightedMixer {
    public static float Mix((float, float)[] weightedValues) {
        var totalWeight = 0f;
        var totalValue = 0f;
        foreach (var (weight, value) in weightedValues) {
            totalWeight += weight;
            totalValue += weight * value;
        }
        if (totalWeight == 0) return 0; // avoid divide by zero
        return totalValue / totalWeight;
    }
}