using System;
namespace RacingProject.Domain
{
    public class Driver
    {
        public string Name {get; private set;}
        public int Skill {get; private set;}
        public int Aggressiveness {get; private set;}

        public int Level {get; private set;}
        public int Experience {get; private set;}
        public int DevelopmentPoints {get; private set;}

        public int ExperienceToNextLevel => Level * 100;
        public double ExperienceProgress => (double)Experience / ExperienceToNextLevel;

        public bool CanUpgradeSkill => DevelopmentPoints > 0 && Skill < 100;
        public bool CanUpgradeAggressiveness=> DevelopmentPoints > 0 && Aggressiveness < 100;

        public Driver(
            string name,
            int skill = 50,
            int aggressiveness = 50
        )
        {
            if (string.IsNullOrWhiteSpace(name))
            
                throw new ArgumentException("Driver's name cannot be empty", nameof(name));
            

            if (skill < 0 || skill > 100)
                throw new ArgumentOutOfRangeException(nameof(skill), "Skill must be from 0 to 100");

            if (aggressiveness < 0 || aggressiveness > 100)
                throw new ArgumentOutOfRangeException(nameof(aggressiveness), "Aggressiveness must be from 0 to 100");

            Name = name;
            Skill = skill;
            Aggressiveness = aggressiveness;

            Level = 1;
            Experience = 0;
            DevelopmentPoints = 0;
        }

        public void GainExperience(int amount)
        {
            if(amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            Experience += amount;

            while(Experience >= ExperienceToNextLevel)
            {
                Experience -= ExperienceToNextLevel;
                Level++;
                DevelopmentPoints++;
            }
        }

        public void UpgradeSkill()
        {
            if(DevelopmentPoints <= 0)
            {
                throw new InvalidOperationException("Not enough development points.");
            }

            if(Skill >= 100)
            {
                throw new InvalidOperationException("Skill is already maximum");
            }

            --DevelopmentPoints;
            Skill++;
        }

        public void UpgradeAggressiveness()
        {
            if(DevelopmentPoints <= 0)
            {
                throw new InvalidOperationException("Not enough development points.");
            }

            if(Aggressiveness >= 100)
            {
                throw new   InvalidOperationException("Aggressiveness is already maximum");
            }

            --DevelopmentPoints;
            Aggressiveness++;
        }

        internal void RestoreProgress(
            int level,
            int experience,
            int developmentPoints
        )
        {
            if(level < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(level));
            }

            if(experience < 0 || experience > level * 100)
            {
                throw new ArgumentOutOfRangeException(nameof(experience));
            }

            if(developmentPoints < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(developmentPoints));
            }

            Level = level;
            Experience = experience;
            DevelopmentPoints = developmentPoints;
        }

        public void GainDevelopmentPoints(int amount)
        {
            DevelopmentPoints += amount;
        }
    }
}