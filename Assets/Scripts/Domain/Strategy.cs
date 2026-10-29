namespace RacingProject.Domain
{
    public sealed class Strategy
    {
        public double TimeMultiplier { get; }
        public double WearMultiplier { get; }
        public double AttackMultiplier { get; } 

        private Strategy(
            double timeMultiplier,
            double wearMultiplier,
            double attackMultiplier
        )
        {
            TimeMultiplier = timeMultiplier;
            WearMultiplier = wearMultiplier;
            AttackMultiplier = attackMultiplier;
        }

        public static readonly Strategy ATTACK = new Strategy(0.95, 1.5, 1.15);

        public static readonly Strategy NORMAL = new Strategy(1.0, 1.0, 1.0);

        public static readonly Strategy CONSERVE = new Strategy(1.05, 0.6, 0.85);
    }
}