namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class Constants {
    public class Facts {
        public const string PERSON_HEALTH = "health";
        public const string PERSON_DRUNKENNESS = "drunkenness";
    }

    public class Values {
        // BAC values
        public const float DANGEROUS_DRUNKENNESS = 0.4f; // getting close to death
        public const float SOBER_ENOUGH = 0.08f; // sober enough to drive
    }
}