using System;
using System.Collections.Generic;
using System.Linq;
using RacingProject.Domain;

namespace RacingProject.Simulation
{
    public class Race
    {
        private readonly List<Team> Teams;
        private readonly Track Track;
        private readonly List<double> Prizes;
        private readonly Random Random;

        private readonly Dictionary<Team, RaceResult> Results = new();
        private readonly List<Team> Positions = new();
        private readonly RaceAI AI = new RaceAI();

        public int CurrentLap {get; private set;}
        public bool IsStarted {get; private set;}

        public int TotalLaps => Track.Laps;

        public Race(
            List<Team> teams,
            Track track,
            List<double> prizes,
            Random random = null
        )
        {
            if (teams == null || teams.Count == 0)
            {
                throw new ArgumentException(
                    "At least one team must participate",
                    nameof(teams)
                );
            }

            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            if (prizes == null)
            {
                throw new ArgumentNullException(nameof(prizes));
            }

            if (teams.Exists(team => team == null))
            {
                throw new ArgumentException("Teams list cannot contain null values", nameof(teams));
            }

            if (teams.Exists(team => team.Driver == null))
            {
                throw new ArgumentException("Every team must have a driver.", nameof(teams));
            }

            Teams = new List<Team>(teams);
            Track = track;
            Prizes = new List<double>(prizes);
            Random = random ?? new Random();
        }

        public double CalculateTime(Car car)
        {
            double baseTime = Track.Length / car.Speed * 3600;
            double difficultyCoefficient = 1 + (Track.Difficulty / 10) * ((100 - car.Handling) / 100);
            double conditionCoefficient = 1 + (100 - car.Condition) / 100;
            double tireCoefficient = 1 / car.Tire.SpeedMultiplier;

            return baseTime * difficultyCoefficient * conditionCoefficient * tireCoefficient;
        }

        public void StartRun()
        {
            Results.Clear();
            Positions.Clear();

            foreach (Team team in Teams)
            {
                Results.Add(team, new RaceResult());
                Positions.Add(team);
            }

            CurrentLap = 0;
            IsStarted = true;
        }

        public void RunLap(Team team)
        {
            EnsureRaceStarted();

            if (!Results.TryGetValue(team, out RaceResult result))
            {
                throw new ArgumentException("Team does not participate in this race", nameof(team));
            }

            result.Time += CalculateTime(team.Car) * team.Strategy.TimeMultiplier;

            team.Car.Tire.Wear(team.Strategy.WearMultiplier);
        }

        public bool RunNextLap()
        {
            EnsureRaceStarted();

            if (CurrentLap >= Track.Laps)
            {
                return false;
            }

            CurrentLap++;

            Dictionary<Team, Strategy> selectedStrategies = new();
            Dictionary<Team, Defence> selectedDefences = new();

            int lapsRemaining = Track.Laps - CurrentLap + 1;

            foreach (Team team in Teams)
            {
                if(team.IsPlayerControlled)
                {
                    selectedStrategies.Add(team, team.Strategy);
                    selectedDefences.Add(team, team.Defence);
                    continue;
                }
                Defence defence = AI.ChooseDefence(team);

                int teamIndex = Positions.IndexOf(team);
                int position = teamIndex + 1;

                double gapToAhead = GetGapToAhead(team);
                double ownLapTime = CalculateTime(team.Car);
                double opponentLapTime = ownLapTime;

                if(teamIndex > 0)
                {
                    Team opponent = Positions[position - 1];
                    opponentLapTime = GetLapTime(opponent);
                }

                Strategy strategy = AI.ChooseStrategy(
                    team,
                    position,
                    lapsRemaining,
                    gapToAhead,
                    ownLapTime,
                    opponentLapTime
                );

                selectedDefences.Add(team, defence);
                selectedStrategies.Add(team, strategy);
            }

            foreach(Team team in Teams)
            {
                team.SetDefence(selectedDefences[team]);
                team.SetStrategy(selectedStrategies[team]);

                RunLap(team);
                AttemptMistake(team);
            }

            ProcessOvertaking();

            if (CurrentLap == Track.Laps)
            {
                foreach (Team team in Teams)
                {
                    team.Car.Wear((int)CalculateWear());
                }

                return true;
            }

            return true;
        }

        private void ProcessOvertaking()
        {
            for (int i = 1; i < Positions.Count(); i++)
            {
                Team attacker = Positions[i];
                Team defender = Positions[i-1];

                double attackerLapTime = CalculateTime(attacker.Car) * attacker.Strategy.TimeMultiplier;
                double defenderLapTime = CalculateTime(defender.Car) * defender.Strategy.TimeMultiplier;

                if (attackerLapTime >= defenderLapTime)
                {
                    continue;
                }

                TryOvertake(attacker, defender);
            }
        }

        public List<RaceResultEntry> FinishRace()
        {
            EnsureRaceStarted();

            if (CurrentLap < Track.Laps)
            {
                throw new InvalidOperationException("The race has now finished yet");
            }

            List<RaceResultEntry> sortedResults = Results
                            .Select(pair => new RaceResultEntry(
                                pair.Key,
                                pair.Value))
                            .OrderBy(entry => entry.Result.Time)
                            .ToList();

            for (int place = 0; place < sortedResults.Count; place++)
            {
                RaceResultEntry entry = sortedResults[place];

                double prize = place < Prizes.Count ? Prizes[place] : 0;
                entry.Result.Prize = prize;

                if (prize > 0)
                {
                    entry.Team.AddMoney(prize);
                }

                int experience = place switch
                {
                    0 => 150,
                    1 => 110,
                    2 => 80,
                    _ => 50
                };

                entry.Team.Driver.GainExperience(experience);
            }

            foreach(RaceResultEntry entry in sortedResults)
            {
                entry.Result.Time = Math.Round(entry.Result.Time, 4);
            }

            IsStarted = false;

            return sortedResults;
        }

        public double PitStop(Team team, Tire newTire)
        {
            EnsureRaceStarted();

            if(!Results.TryGetValue(team, out RaceResult result))
            {
                throw new ArgumentException("Team does not participate in this race", nameof(team));
            }

            team.Car.ChangeTire(newTire);

            const double pitStopTime = 20;

            result.Time += pitStopTime;

            return pitStopTime;
        }

        public double CalculateWear()
        {
            return Track.GetTotalDistance() * 0.05 * (1 + Track.Difficulty / 10);
        }

        public double CalculateOvertakingChance(
            Team attacker,
            Team defender
        )
        {
            if (attacker == null || defender == null)
            {
                throw new ArgumentNullException("Both teams are required");
            }

            double attackScore = attacker.Car.Speed * 0.4 +
                                 attacker.Car.Handling * 0.3 +
                                 attacker.Driver.Skill * 0.2 +
                                 attacker.Driver.Aggressiveness * 0.1;

            double defenceScores = defender.Car.Speed * 0.4 +
                                 defender.Car.Handling * 0.3 +
                                 defender.Driver.Skill * 0.2 +
                                 defender.Defence.DefenceBonus;

            attackScore *= attacker.Strategy.AttackMultiplier;

            double difference = attackScore - defenceScores;

            double chance = (difference + 15) / 30.0;

            return Math.Max(0, Math.Min(1, chance));
        }

        public string AttemptOvertaking(Team attackingTeam, Team defendingTeam)
        {
            double chance = CalculateOvertakingChance(attackingTeam, defendingTeam);

            return Random.NextDouble() < chance ? "success" : "failed";
        }

        public double CalculateMistakeChance(Team team)
        {
            double mistakeChance = 0.15 + (100 - team.Driver.Skill) / 100.0 * 0.10;

            double tirePenalty = (100 - team.Car.Tire.Condition) / 100.0 * 0.10;

            double carPenalty = (100 - team.Car.Condition) / 100.0 * 0.10;

            mistakeChance += tirePenalty + carPenalty;

            mistakeChance *= 1 + team.Driver.Aggressiveness / 100.0 * 0.25;

            if (ReferenceEquals(team.Strategy, Strategy.ATTACK))
            {
                mistakeChance *= 1.5;
            }
            else if (ReferenceEquals(team.Strategy, Strategy.CONSERVE))
            {
                mistakeChance *= 0.6;
            }

            mistakeChance *= 1 + team.Defence.MistakeRisk;

            return Math.Max(0, Math.Min(0.75, mistakeChance));
        }

        public bool AttemptMistake(Team team)
        {
            EnsureRaceStarted();
            
            if(!Results.TryGetValue(team, out RaceResult result))
                {
                    throw new ArgumentException("Team does not participate in this race", nameof(team));
                }
    
            double mistakeChance = CalculateMistakeChance(team);

            if(Random.NextDouble() < mistakeChance)
            {
                double lostTime = 2 + Random.NextDouble() * 6;
                result.Time += lostTime;

                return true;
            }

            return false;
        }

        public RaceResult GetResult(Team team)
        {
            if (!Results.TryGetValue(team, out RaceResult result))
            {
                throw new ArgumentException("Team does not participate in this race.", nameof(team));
            }

            return result;
        }
        
        public List<Team> GetPositions()
        {
            EnsureRaceStarted();

            return new List<Team>(Positions);
        }
        
        public bool TryOvertake(Team attacker, Team defender)
        {
            EnsureRaceStarted();

            if (attacker == null)
            {
                throw new ArgumentNullException(nameof(attacker));
            }

            if (defender == null)
            {
                throw new ArgumentNullException(nameof(defender));
            }

            int attackerPosition = Positions.IndexOf(attacker);
            int defenderPosition = Positions.IndexOf(defender);

            if (attackerPosition < 0 || defenderPosition < 0)
            {
                throw new ArgumentException("Both teams  must participate in this race.");
            }

            if (attackerPosition != defenderPosition + 1)
            {
                return false;
            }

            string result = AttemptOvertaking(attacker, defender);

            if (result != "success")
            {
                return false;
            }

            
            (Positions[attackerPosition], Positions[defenderPosition]) =
                (Positions[defenderPosition], Positions[attackerPosition]);

            return true;
        }

        public void EnsureRaceStarted()
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException("Start the race first");
            }
        }

        private double GetGapToAhead(Team team)
        {
            int position = Positions.IndexOf(team);

            if(position < 0)
            {
                throw new ArgumentException(nameof(team));
            }

            if(position == 0)
            {
                return 0;
            }

            Team ahead = Positions[position - 1];

            double teamTime = Results[team].Time;
            double aheadTime = Results[ahead].Time;

            return Math.Max(0, teamTime - aheadTime);
        }

        private double GetLapTime(Team team)
        {
            return CalculateTime(team.Car) * team.Strategy.TimeMultiplier;
        }
    }

    public class RaceResult
    {
        public double Time {get; set;}
        public double Prize {get; set;}
    }

    public class RaceResultEntry
    {
        public Team Team {get;}
        public RaceResult Result {get;}

        public RaceResultEntry(
            Team team, RaceResult result
        )
        {
            Team = team;
            Result = result;
        }
    }
}