namespace RacingProject.Domain
{
    public class Soft : Tire
    {
        public override string Name => "Soft";
        public override double SpeedMultiplier => 1 + 0.08 * Condition / 100;
        public override double WearPerLap => 4;
    }
}