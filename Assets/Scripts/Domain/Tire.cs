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

            Condition = Math.Round(Math.Max(0, Condition - WearPerLap * multiplier), 2);            
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

        public void WearForDistance(
            double distance,
            double trackLength,
            double multiplier = 1)
        {
            if (double.IsNaN(distance) || double.IsInfinity(distance) || distance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(distance));
            }

            if (double.IsNaN(trackLength) || double.IsInfinity(trackLength) || trackLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(trackLength));
            }

            if (double.IsNaN(multiplier) || double.IsInfinity(multiplier) || multiplier < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier));
            }

            double distanceFraction = distance / trackLength;

            Condition = Math.Round(Math.Max(0, Condition - WearPerLap * distanceFraction * multiplier), 2);
        }
    }
}