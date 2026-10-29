namespace RacingProject.Domain
{
    public sealed class Defence
    {
        public double DefenceBonus { get; }
        public double MistakeRisk { get; }

        private Defence(
            double defenceBonus,
            double mistakeRisk
        )
        {
            DefenceBonus = defenceBonus;
            MistakeRisk = mistakeRisk;
        }

        public static readonly Defence NORMAL = new Defence(0, 0);
        public static readonly Defence AGGRESSIVE = new Defence(10, 0.15);
        public static readonly Defence YIELD = new Defence(-10, 0);
    }
}