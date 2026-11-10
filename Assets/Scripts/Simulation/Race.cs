using System;
using System.Collections.Generic;
using System.Linq;
using RacingProject.Domain;
using UnityEngine;

namespace RacingProject.Simulation
{
    public class Race
    {
        private readonly List<Team> Teams;
        private readonly Track Track;
        private readonly List<double> Prizes;
        private readonly System.Random Random;

        private readonly List<double> StartingDistances;
        private readonly Dictionary<Team, RaceResult> Results = new();
        private readonly Dictionary<Team, TeamRaceProgress> TeamProgress = new();
        private readonly List<Team> Positions = new();
        private readonly RaceAI AI = new RaceAI();
        private readonly HashSet<Team> notifiedOpponents= new();

        public int CurrentLap {get; private set;}
        public bool IsStarted {get; private set;}

        public int TotalLaps => Track.Laps;

        public RaceEvent PendingEvent { get; private set; }

        public Race(
            List<Team> teams,
            Track track,
            List<double> prizes,
            System.Random random = null,
            List<double> startingDistances = null
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
            Random = random ?? new System.Random();

            if(startingDistances != null && startingDistances.Count != Teams.Count)
            {
                throw new ArgumentException("Starting distances count must match teams count",
                    nameof(startingDistances));
            }

            StartingDistances = startingDistances != null
                ? new List<double>(startingDistances) : new List<double>(new double[teams.Count]);

            double totalDistance = track.GetTotalDistance();

            foreach(double distance in StartingDistances)
            {
                if(distance < 0 || distance > totalDistance)
                {
                    throw new ArgumentOutOfRangeException(nameof(startingDistances));
                }
            }
        }

        public double CalculateTime(Car car)
        {
            double baseTime = Track.Length / car.Speed * 3600;
            double difficultyCoefficient = 1 + (Track.Difficulty / 10) * ((100 - car.Handling) / 100);
            double conditionCoefficient = 1 + (100 - car.Condition) / 100;
            double tireCoefficient = 1 / car.Tire.SpeedMultiplier;

            return baseTime * difficultyCoefficient * conditionCoefficient * tireCoefficient;
        }

        public double CalculateDistance(Team team, double elapsedTime)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }

            if (elapsedTime <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedTime));
            }

            double laptime = CalculateTime(team.Car) * team.Strategy.TimeMultiplier;

            if(laptime <= 0)
            {
                throw new InvalidOperationException("Lap time must be positive");
            }

            return Track.Length / laptime * elapsedTime;
        }

        public void StartRun()
        {
            Results.Clear();
            Positions.Clear();
            TeamProgress.Clear();
            notifiedOpponents.Clear();
            PendingEvent = null;

            for (int i = 0; i < Teams.Count; i++)
            {
                Team team = Teams[i];

                Results.Add(team, new RaceResult());
                Positions.Add(team);

                TeamProgress.Add(
                    team,
                    new TeamRaceProgress(
                        Track.Length,
                        Track.Laps,
                        StartingDistances[i])
                );
            }

            CurrentLap = 0;
            IsStarted = true;
        }

        public void RunLap(Team team)
        {
            EnsureRaceStarted();

            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }

            if (!Results.ContainsKey(team))
            {
                throw new ArgumentException("Team doesn't participate in this race.", nameof(team));
            }

            double lapTime = GetLapTime(team);

            AdvanceTeam(team, lapTime);
        }

        private void AdvanceTeam(Team team, double elapsedTime)
        {
            TeamRaceProgress progress = TeamProgress[team];

            if(progress.IsFinished)
            {
                return;
            }

            double distance = CalculateDistance(team, elapsedTime);
            double previousDistance = progress.DistanceCovered;

            progress.Advance(distance, elapsedTime);

            double actualDistance = progress.DistanceCovered - previousDistance;
            double actualTime = elapsedTime * (actualDistance / distance);

            Results[team].Time += actualTime;
            team.Car.Tire.WearForDistance(distance, Track.Length, team.Strategy.WearMultiplier);
        }

        public bool RunNextLap()
        {
            EnsureRaceStarted();

            if (Teams.All(team => TeamProgress[team].IsFinished))
            {
                return false;
            }

            CurrentLap = Math.Min(CurrentLap + 1, Track.Laps);

            Dictionary<Team, Strategy> selectedStrategies = new();
            Dictionary<Team, Defence> selectedDefences = new();

            int lapsRemaining = Track.Laps - CurrentLap + 1;

            foreach (Team team in Teams)
            {
                if (TeamProgress[team].IsFinished)
                {
                    continue;
                }

                if(team.IsPlayerControlled)
                {
                    selectedStrategies.Add(team, team.Strategy);
                    continue;
                }

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

                selectedStrategies.Add(team, strategy);
            }

            foreach(Team team in Teams)
            {
                if (TeamProgress[team].IsFinished)
                {
                    continue;
                }

                team.SetStrategy(selectedStrategies[team]);
            }

            foreach (Team team in Teams)
            {
                if (TeamProgress[team].IsFinished)
                    continue;

                double lapTime = GetLapTime(team);

                AdvanceTeam(team, lapTime);
                AttemptMistake(team);
            }

            UpdatePosition();

            CheckPlayerProximity();

            return true;
        }

        public void AdvanceSimulationStep(double elapsedTime) 
        {
            EnsureRaceStarted();

            if(elapsedTime <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedTime));
            }

            const double timeStep = 0.1;

            double remainingTime = elapsedTime;

            while(remainingTime > 0)
            {
                double currentStep = Math.Min(timeStep, remainingTime);

                Dictionary<Team, double> previousDistance = new();

                foreach (Team team in Teams)
                {
                    previousDistance[team] = TeamProgress[team].DistanceCovered;

                    TeamRaceProgress progress = TeamProgress[team];

                    if (progress.IsFinished)
                    {
                        continue;
                    }

                    double distance = CalculateDistance(team, currentStep);
                    progress.Advance(distance, currentStep);

                    double actualDistance = progress.DistanceCovered - previousDistance[team];
                    double actualTime = currentStep * (actualDistance / distance);

                    Results[team].Time += actualTime;
                }

                remainingTime -= currentStep;
            }
            UpdatePosition();
        }

        public List<RaceResultEntry> FinishRace()
        {
            EnsureRaceStarted();

            if (CurrentLap < Track.Laps)
            {
                throw new InvalidOperationException("The race has not finished yet");
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
                entry.Result.Experience = experience;
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

            if (TeamProgress[team].IsFinished)
            {
                throw new InvalidOperationException("Finished teams cannot perform pit stops");
            }

            team.Car.ChangeTire(newTire);

            const double pitStopTime = 20;

            result.Time += pitStopTime;

            UpdatePosition();

            return pitStopTime;
        }

        public double CalculateWear()
        {
            return Track.GetTotalDistance() * 0.05 * (1 + Track.Difficulty / 10);
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

        
        private void UpdatePosition()
        {
            Positions.Sort((teamA, teamB) =>
            {
                double timeA = Results[teamA].Time;
                double timeB = Results[teamB].Time;

                return timeA.CompareTo(timeB);
            });
        }

        public void EnsureRaceStarted()
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException("Start the race first");
            }
        }

        public TeamRaceProgress GetTeamProgress(Team team)
        {
            if(!TeamProgress.TryGetValue(team, out TeamRaceProgress progress))
            {
                throw new ArgumentException("Team does not participate in race", nameof(team));
            }

            return progress;
        }

        public double GetGapToAhead(Team team)
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

            return Results[team].Time - Results[ahead].Time;
        }

        private void CheckPlayerProximity()
        {
            Team playerTeam = Teams.FirstOrDefault(team => team.IsPlayerControlled);

            if (playerTeam == null)
            {
                return;
            }

            int playerPosition = Positions.IndexOf(playerTeam);
            if(playerPosition == 0)
            {
                return;
            }

            Team opponentAhead = Positions[playerPosition - 1];
            double gap = GetGapToAhead(playerTeam);

            const double proximityThreshold = 2.0;

            if (gap <= proximityThreshold &&
                !notifiedOpponents.Contains(opponentAhead) &&
                PendingEvent == null)
            {
                PendingEvent = new RaceEvent(RaceEventType.CloseToOpponent,
                    $"Вы приближаетесь к команде {opponentAhead.Name}! Попробуйте использовать атакующую стратегию");

                notifiedOpponents.Add(opponentAhead);
            }
        }

        private double GetLapTime(Team team)
        {
            return CalculateTime(team.Car) * team.Strategy.TimeMultiplier;
        }

        public void AcknowledgeEvent()
        {
            PendingEvent = null;
        }
    }

    public class RaceResult
    {
        public double Time {get; set;}
        public double Prize {get; set;}
        public int Experience { get; set; }
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