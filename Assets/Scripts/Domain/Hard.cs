namespace RacingProject.Domain
{
    public class Hard : Tire
    {
        public override string Name => "Hard";
        public override double SpeedMultiplier => 1;
        public override double WearPerLap => 1;
    }
}