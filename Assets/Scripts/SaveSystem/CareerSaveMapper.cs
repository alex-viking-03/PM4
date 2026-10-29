using System;
using RacingProject.Domain;
using System.Linq;

namespace RacingProject.SaveSystem
{
    public static class CareerSaveMapper
    {
        public static CareerSaveData CreateSaveData(Team team)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }

            if (team.Car == null)
            {
                throw new InvalidOperationException(
                    $"Team '{team.Name}' has no car.");
            }

            if (team.Car.Tire == null)
            {
                throw new InvalidOperationException(
                    $"Car '{team.Car.Name}' has no tires.");
            }

            if (team.Driver == null)
            {
                throw new InvalidOperationException(
                    $"Team '{team.Name}' has no driver.");
            }

            if (team.Strategy == null)
            {
                throw new InvalidOperationException(
                    $"Team '{team.Name}' has no strategy.");
            }

            if (team.Defence == null)
            {
                throw new InvalidOperationException(
                    $"Team '{team.Name}' has no defence.");
            }

            return new CareerSaveData
            {
                SaveVersion = 1,
                TeamName = team.Name,
                Budget = team.Budget,

                IsPlayerControlled = team.IsPlayerControlled,
                StrategyName = GetStrategyName(team.Strategy),
                DefenceName = GetDefenceName(team.Defence),

                Car = new CarSaveData
                {
                    Name = team.Car.Name,
                    Speed = team.Car.Speed,
                    Handling = team.Car.Handling,
                    Condition = team.Car.Condition,

                    TireName = team.Car.Tire.Name,
                    TireCondition = team.Car.Tire.Condition,

                    SpeedLevel = team.Car.SpeedLevel,
                    HandlingLevel = team.Car.HandlingLevel
                },
                Driver = new DriverSaveData
                {
                    Name = team.Driver.Name,
                    Skill = team.Driver.Skill,
                    Aggressiveness = team.Driver.Aggressiveness,

                    Level = team.Driver.Level,
                    Experience = team.Driver.Experience,
                    DevelopmentPoints = team.Driver.DevelopmentPoints
                },

            };
        }

        public static Team RestoreTeam(CareerSaveData data)
        {
            ValidateSaveData(data);

            Tire tire = CreateTire(
                data.Car.TireName,
                data.Car.TireCondition
            );

            Car car = new Car(
                data.Car.Name,
                data.Car.Speed,
                data.Car.Handling,
                tire,
                data.Car.Condition
            );

            car.RestoreUpgradeLevels(
                data.Car.SpeedLevel,
                data.Car.HandlingLevel
            );

            Driver driver = new Driver(
                data.Driver.Name,
                data.Driver.Skill,
                data.Driver.Aggressiveness
            );

            driver.RestoreProgress(
                data.Driver.Level,
                data.Driver.Experience,
                data.Driver.DevelopmentPoints
            );

            return new Team(
                data.TeamName,
                car,
                data.Budget,
                GetStrategy(data.StrategyName),
                driver,
                GetDefence(data.DefenceName),
                data.IsPlayerControlled
            );
        }

        private static void ValidateSaveData(CareerSaveData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.SaveVersion != 1)
                throw new ArgumentException("Unsupported team save version.");

            if (string.IsNullOrWhiteSpace(data.TeamName))
                throw new ArgumentException("Team name is missing.");

            if (double.IsNaN(data.Budget) ||
                double.IsInfinity(data.Budget) ||
                data.Budget < 0)
                throw new ArgumentException("Team budget is invalid.");

            if (data.Car == null || data.Driver == null)
                throw new ArgumentException("Car or driver data is missing.");

            if (string.IsNullOrWhiteSpace(data.Car.Name))
                throw new ArgumentException("Car name is missing.");

            if (double.IsNaN(data.Car.Speed) ||
                double.IsInfinity(data.Car.Speed) ||
                data.Car.Speed <= 0 || data.Car.Speed > 100)
                throw new ArgumentException("Car speed is invalid.");

            if (double.IsNaN(data.Car.Handling) ||
                double.IsInfinity(data.Car.Handling) ||
                data.Car.Handling <= 0 || data.Car.Handling > 100)
                throw new ArgumentException("Car handling is invalid.");

            if (double.IsNaN(data.Car.Condition) ||
                double.IsInfinity(data.Car.Condition) ||
                data.Car.Condition < 0 || data.Car.Condition > 100)
                throw new ArgumentException("Car condition is invalid.");

            if (double.IsNaN(data.Car.TireCondition) ||
                double.IsInfinity(data.Car.TireCondition) ||
                data.Car.TireCondition < 0 || data.Car.TireCondition > 100)
                throw new ArgumentException("Tire condition is invalid.");

            if (string.IsNullOrWhiteSpace(data.Car.TireName))
                throw new ArgumentException("Tire type is missing.");

            if (data.Car.SpeedLevel < 0 || data.Car.SpeedLevel > 3 ||
                data.Car.HandlingLevel < 0 || data.Car.HandlingLevel > 3)
                throw new ArgumentException("Car upgrade levels are invalid.");

            if (string.IsNullOrWhiteSpace(data.Driver.Name))
                throw new ArgumentException("Driver name is missing.");

            if (data.Driver.Skill < 0 || data.Driver.Skill > 100 ||
                data.Driver.Aggressiveness < 0 ||
                data.Driver.Aggressiveness > 100)
                throw new ArgumentException("Driver attributes are invalid.");

            if (data.Driver.Level < 1 ||
                data.Driver.Experience < 0 ||
                data.Driver.DevelopmentPoints < 0)
                throw new ArgumentException("Driver progress is invalid.");

            if (data.Driver.Experience >= data.Driver.Level * 100)
                throw new ArgumentException("Driver experience exceeds the current level threshold.");

            _ = GetStrategy(data.StrategyName);
            _ = GetDefence(data.DefenceName);

            if (data.Car.TireName != "Soft" &&
                data.Car.TireName != "Medium" &&
                data.Car.TireName != "Hard")
                throw new ArgumentException(
                    $"Unknown tire type: {data.Car.TireName}");
        }

        private static Tire CreateTire(
            string name,
            double condition
        )
        {
            Tire tire = name switch
            {
                "Soft" => new Soft(),
                "Medium" => new Medium(),
                "Hard" => new Hard(),
                _ => throw new ArgumentException($"Unknown tire type: {name}")
            };

            tire.RestoreCondition(condition);

            return tire;
        }

        private static string GetStrategyName(Strategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            if (strategy.TimeMultiplier == 0.95 &&
                strategy.WearMultiplier == 1.5 &&
                strategy.AttackMultiplier == 1.15)
                return "ATTACK";

            if (strategy.TimeMultiplier == 1.0 &&
                strategy.WearMultiplier == 1.0 &&
                strategy.AttackMultiplier == 1.0)
                return "NORMAL";

            if (strategy.TimeMultiplier == 1.05 &&
                strategy.WearMultiplier == 0.6 &&
                strategy.AttackMultiplier == 0.85)
                return "CONSERVE";

            throw new ArgumentException("Unknown strategy.");
        }

        private static Strategy GetStrategy(string name)
        {
            return name switch
            {
                "ATTACK" => Strategy.ATTACK,
                "NORMAL" => Strategy.NORMAL,
                "CONSERVE" => Strategy.CONSERVE,
                _ => throw new ArgumentException(
                    $"Unknown strategy: {name}")
            };
        }

        private static string GetDefenceName(Defence defence)
        {
            if (defence == null)
                throw new ArgumentNullException(nameof(defence));

            if (defence.DefenceBonus == 0 &&
                defence.MistakeRisk == 0)
                return "NORMAL";

            if (defence.DefenceBonus == 10 &&
                defence.MistakeRisk == 0.15)
                return "AGGRESSIVE";

            if (defence.DefenceBonus == -10 &&
                defence.MistakeRisk == 0)
                return "YIELD";

            throw new ArgumentException("Unknown defence type.");
        }

        private static Defence GetDefence(string name)
        {
            return name switch
            {
                "NORMAL" => Defence.NORMAL,
                "AGGRESSIVE" => Defence.AGGRESSIVE,
                "YIELD" => Defence.YIELD,
                _ => throw new ArgumentException(
                    $"Unknown defence type: {name}")
            };
        }

        public static FullCareerSaveData CreateFullCareerSaveData(
            System.Collections.Generic.List<Team> teams,
            int completedRaces
        )
        {
            if(teams == null || teams.Count == 0)
            {
                throw new ArgumentException("Career has no teams", nameof(teams));
            }

            if(teams.Exists(team => team == null))
            {
                throw new ArgumentException("Career contains an empty team");
            }

            int playerTeamCount = teams.Count(team => team.IsPlayerControlled);

            if (playerTeamCount != 1)
            {
                throw new ArgumentException("Career must contain exactly one player team");
            }

            if(completedRaces < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(completedRaces));
            }

            return new FullCareerSaveData{
                SaveVersion = 1,
                CompletedRaces = completedRaces,
                Teams = teams.ConvertAll(CreateSaveData)
            };
        }

        public static System.Collections.Generic.List<Team> RestoreTeams(
            FullCareerSaveData data
        )
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if(data.Teams == null || data.Teams.Count == 0)
            {
                throw new ArgumentException("Save contains no teams", nameof(data));
            }

            if(data.Teams.Exists(teamData => teamData == null))
            {
                throw new ArgumentException("Save contains an empty team entry", nameof(data));
            }

            return data.Teams.ConvertAll(RestoreTeam);
        }
    }
}