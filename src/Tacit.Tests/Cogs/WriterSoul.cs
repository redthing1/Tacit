using Tacit.Calc;
using Tacit.Cogs;

namespace Tacit.Tests.Cogs;

public class WriterSoul : Sentient<WriterSoul.Personality, WriterSoul.Traits, WriterSoul.Emotions> {
    public class Personality : Tacit.Cogs.Personality {
        /// <summary>
        ///     creativity
        /// </summary>
        public float c;

        /// <summary>
        ///     extraversion
        /// </summary>
        public float e;

        public Personality() {}

        public Personality(float e, float c) {
            this.e = e;
            this.c = c;
        }

        public override float[] Vec => new[] { e, c };

        public static Personality MakeRandom() {
            // generate personalities along a normal distribution
            return new Personality(
                PerMath.Clamp11(NormalRand(u: 0.2f, s: 0.6f)),// extraversion distribution
                PerMath.Clamp11(NormalRand(u: -0.2f, s: 0.6f))// creativity distribution
            );
        }
    }

    public class Traits : Traits<Personality> {
        public static float[] vecProductivity = { 0.5f, -0.2f };
        public static float[] vecPopularity = { 0.9f, 0.4f };
        public float popularity;

        public float productivity;

        public override void Calculate(Personality ply) {
            productivity = VectorTrait.RawValue(vecProductivity, ply);
            popularity = VectorTrait.NormalizedValue(vecPopularity, ply);
        }
    }

    public class Emotions : Tacit.Cogs.Emotions {
        // emotions: [inspired, happy]
        public override float[] Vec { get; } = { 0f, 0f };
        public override float Falloff => 0.9f;

        public ref float Inspired => ref Vec[0];
        public ref float Happy => ref Vec[1];
    }
}