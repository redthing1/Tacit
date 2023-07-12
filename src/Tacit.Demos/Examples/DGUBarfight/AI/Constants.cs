namespace Tacit.Demos.Examples.DGUBarfight.AI;

public class Constants {
    public class Facts {
        public const string PERSON_HEALTH = "health";
        public const string PERSON_DRUNKENNESS = "drunkenness";
        
        public const string ALL_PERSONS = "all_persons";
    }

    public class Values {
        // HEALTH values
        public const float HEALTH_LOW = 0.4f; // low health
        public const float HEALTH_MAX = 1f; // max health
        public const float BASE_PUNCH_DAMAGE = 0.1f; // damage from a punch
        // BAC values
        public const float DRUNKENNESS_IMPAIRED = 0.25f; // impaired
        public const float DANGEROUS_DRUNKENNESS = 0.4f; // getting close to death
        public const float DEADLY_DRUNKENNESS = 0.5f; // death
        public const float SOBER_ENOUGH = 0.08f; // sober enough to drive
        
        public const float TYPICAL_GLASS_VOLUME = 250; // typical glass at this bar is 250mL
        public const float BEER_ABV = 5; // beer is about 5% alcohol
        public const float WINE_ABV = 12; // wine is about 12% alcohol
        public const float LIQUOR_ABV = 40; // liquor is about 40% alcohol
        public const float BASE_BREAD_BAC_REDUCTION = 0.01f; // bread reduces BAC somewhat
        public const float HEAL_FROM_DRINKING_GLASS = 0.25f; // drinking a glass of alcohol heals you a bit
    }
}