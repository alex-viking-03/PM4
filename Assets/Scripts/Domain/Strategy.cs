namespace RacingProject.Domain
{
    public sealed class Strategy
    {
        public string Name { get; set; }

        public double TimeMultiplier { get; }
        public double WearMultiplier { get; }
        public double AttackMultiplier { get; } 

        private Strategy(
            string name,
            double timeMultiplier,
            double wearMultiplier,
            double attackMultiplier
        )
        {
            Name = name;

            TimeMultiplier = timeMultiplier;
            WearMultiplier = wearMultiplier;
            AttackMultiplier = attackMultiplier;
        }

        public static readonly Strategy ATTACK = new Strategy("Атака", 0.95, 1.5, 1.15);

        public static readonly Strategy NORMAL = new Strategy("Баланс", 1.0, 1.0, 1.0);

        public static readonly Strategy CONSERVE = new Strategy("Защита", 1.05, 0.6, 0.85);
    }
}