using System;

namespace RacingProject.Domain
{
    public class Car
    {
        public string Name {get; private set;}
        public double Speed {get; private set;}
        public double Handling {get; private set;}
        public double Condition {get; private set;}
        public Tire Tire {get; private set;}
        public int SpeedLevel {get; private set;}
        public int HandlingLevel {get; private set;}

        public Car(
            string name,
            double speed,
            double handling,
            Tire tire,
            double condition = 100
        )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Car name cannot be empty");

            if (speed <= 0)
                throw new ArgumentOutOfRangeException(nameof(speed));

            if (handling <= 0)
                throw new ArgumentOutOfRangeException(nameof(handling));

            if (condition < 0 || condition > 100)
                    throw new ArgumentOutOfRangeException(nameof(condition));

            if (tire == null)
                throw new ArgumentNullException(nameof(tire));

            Name = name;
            Speed = speed;
            Handling = handling;
            Condition = condition;
            Tire = tire;
        }

        public void Wear(double amount)
        {
                if(amount < 0)
                    throw new ArgumentOutOfRangeException(nameof(amount));

                Condition = System.Math.Max(0, Condition - amount);
        }

        public void Repair()
        {
            Condition = 100;
        }

        public void UpgradeSpeed(double amount)
        {
            if(amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            if(Speed + amount > 100)
            {
                throw new InvalidOperationException("Speed cannot be more than 100.");
            }

            Speed += amount;
            SpeedLevel++;
        }

        public void UpgradeHandling(double amount)
        {
            if(amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            if(Handling + amount > 100)
            {
                throw new InvalidOperationException("Handling cannot be more than 100.");
            }

            Handling += amount;
            HandlingLevel++;
        }

        public void ChangeTire(Tire newTire)
        {
            if (newTire == null)
            {
                throw new ArgumentNullException(nameof(newTire));
            }

            Tire = newTire;
        }

        internal void RestoreUpgradeLevels(
            int speedLevel,
            int handlingLevel
        )
        {
            if(speedLevel < 0 || speedLevel > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(speedLevel));
            }

            if(handlingLevel < 0 || handlingLevel > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(handlingLevel));
            }

            SpeedLevel = speedLevel;
            HandlingLevel = handlingLevel;
        }
    }
}