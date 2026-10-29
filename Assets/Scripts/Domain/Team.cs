namespace RacingProject.Domain
{
    public class Team
    {
        public string Name{get; private set;}
        public Car Car {get; private set;}
        public double Budget {get; private set;}
        public Strategy Strategy {get; private set;}
        public Driver Driver {get; private set;}
        public Defence Defence {get; private set;}

        public double NextSpeedUpgradeCost => GetSpeedUpgradeCost();
        public double NextHandlingUpgradeCost => GetHandlingUpgradeCost();

        public double NextSpeedUpgradeAmount => GetUpgradeAmount(Car.SpeedLevel);
        public double NextHandlingUpgradeAmount => GetUpgradeAmount(Car.HandlingLevel);

        public bool IsPlayerControlled {get; private set;}

        public Team(
            string name,
            Car car,
            double budget,
            Strategy strategy = null,
            Driver driver = null,
            Defence defence = null,
            bool isPlayerControlled = false
        )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("Team name cannot be empty", nameof(name));

            if(car == null)
                throw new System.ArgumentNullException(nameof(car));

            if (budget < 0)
                throw new System.ArgumentOutOfRangeException(nameof(budget));


            Name = name;
            Car = car;
            Budget = budget;
            Strategy = strategy ?? Strategy.NORMAL;
            Driver = driver;
            Defence = defence ?? Defence.NORMAL;
            IsPlayerControlled = isPlayerControlled;
        }

        public double GetRepairCost()
        {
            return System.Math.Ceiling((100 - Car.Condition) * 100);
        }

        public bool RepairCar()
        {
            double cost = GetRepairCost();

            if (cost > Budget)
                return false;
            
            Car.Repair();

            Budget -= cost;

            return true;
        }

        public void AddMoney(double money)
        {
            if (money <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(money));
            }

            Budget += money;
        }

        public double GetSpeedUpgradeCost()
        {
            return 1000 * System.Math.Pow(3, Car.SpeedLevel + 1);
        }

        public double GetHandlingUpgradeCost()
        {
            return 800 * System.Math.Pow(3, Car.HandlingLevel + 1);
        }

        public bool BuySpeedUpgrade()
        {
            double amount = GetUpgradeAmount(Car.SpeedLevel);
            double cost = GetSpeedUpgradeCost();

            if (cost > Budget || Car.Speed + amount > 100)
            {
                return false;
            }

            Budget -= cost;
            Car.UpgradeSpeed(amount);

            return true;
        }

        public bool BuyHandlingUpgrade()
        {
            double amount = GetUpgradeAmount(Car.HandlingLevel);
            double cost = GetHandlingUpgradeCost();

            if (cost > Budget || Car.Handling + amount > 100)
            {
                return false;
            }

            Budget -= cost;
            Car.UpgradeHandling(amount);

            return true;
        }

        public void SetDefence(Defence defence)
        {
            if (defence == null)
            {
                throw new System.ArgumentNullException(nameof(defence));
            }

            Defence = defence;
        }

        public void SetStrategy(Strategy strategy)
        {
            if (strategy == null)
            {
                throw new System.ArgumentNullException(nameof(strategy));
            }

            Strategy = strategy;
        }

        private double GetUpgradeAmount(int currentLevel)
        {
            return currentLevel switch
            {
                0 => 3,
                1 => 5,
                2 => 8,
                _ => 0
            };
        }
    }
}