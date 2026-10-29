using System;
using System.Collections.Generic;

namespace RacingProject.SaveSystem
{
    [Serializable]
    public class CareerSaveData
    {
        public int SaveVersion = 1;
        public string TeamName;
        public double Budget;

        public CarSaveData Car = new();
        public DriverSaveData Driver = new();

        public string StrategyName;
        public string DefenceName;

        public bool IsPlayerControlled;
    }

    [Serializable]
    public class CarSaveData
    {
        public string Name;
        public double Speed;
        public double Handling;
        public double Condition;

        public string TireName;
        public double TireCondition;

        public int SpeedLevel;
        public int HandlingLevel;
    }

    [Serializable]
    public class DriverSaveData
    {
        public string Name;
        public int Skill;
        public int Aggressiveness;

        public int Level;
        public int Experience;
        public int DevelopmentPoints;
    }

    [Serializable]
    public class FullCareerSaveData
    {
        public int SaveVersion = 1;
        public int CompletedRaces;
        public List<CareerSaveData> Teams = new();
    }
}