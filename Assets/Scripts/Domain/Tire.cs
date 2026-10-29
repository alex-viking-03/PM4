using System;

namespace RacingProject.Domain
{
    public abstract class Tire
    {
        public double Condition { get; private set; } = 100;
        
        public abstract double SpeedMultiplier { get; }
        public abstract string Name { get; }
        public abstract double WearPerLap { get; }

        public void Wear(double multiplier = 1)
        {
            if (multiplier < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier));
            }

            Condition = Math.Round(
                Math.Max(0, Condition - WearPerLap * multiplier), 0);            
        }

        internal void RestoreCondition(double condition)
        {
            if(double.IsNaN(condition) ||
               double.IsInfinity(condition) ||
               condition < 0 || condition > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(condition));
            }

            Condition = condition;
        }
    }
}