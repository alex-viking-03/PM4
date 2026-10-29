namespace RacingProject.Domain
{
    public class Medium : Tire
    {
        public override string Name => "Medium";
        public override double SpeedMultiplier => 1 + 0.04 * Condition / 100;
        public override double WearPerLap => 2;
    }
}