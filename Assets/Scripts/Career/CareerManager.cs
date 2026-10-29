using System;
using System.Collections.Generic;
using System.Linq;
using RacingProject.Domain;
using RacingProject.Simulation;
using RacingProject.SaveSystem;

namespace RacingProject.Career
{
    public class CareerManager
    {
        public Team PlayerTeam {get; private set;}
        public List<Team> Teams {get; private set;} = new();
        public Race CurrentRace {get; private set;}

        public int  CompletedRaces {get; private set;}
        public bool HasActiveRace => CurrentRace != null && CurrentRace.IsStarted;

        public void StartNewCareer()
        {
            PlayerTeam = CreatePlayerTeam();

            Teams = CreateOpponentTeams();
            Teams.Insert(0, PlayerTeam);

            CompletedRaces = 0;
            CurrentRace = null;
        }

        public void StartNextRace(
            Track track,
            List<double> prizes
        )
        {
            if(PlayerTeam == null)
            {
                throw new InvalidOperationException("Start career first");
            }

            if(HasActiveRace)
            {
                throw new InvalidOperationException("Last race is not finished");
            }

            CurrentRace = new Race(Teams, track, prizes);
            CurrentRace.StartRun();
        }

        public bool RunNextLap()
        {
            if(!HasActiveRace)
            {
                throw new InvalidOperationException("Start tace first");
            }

            return CurrentRace.RunNextLap();
        }

        public List<RaceResultEntry> FinishCurrentRace()
        {
            if(CurrentRace == null || !CurrentRace.IsStarted)
            {
                throw new InvalidOperationException("No active race");
            }

            if(CurrentRace.CurrentLap == 0)
            {
                throw new InvalidOperationException("Finishing race before the first lap is impossible");
            }

            if(CurrentRace.CurrentLap < CurrentRace.TotalLaps)
            {
                throw new InvalidOperationException("Race is not finished");
            }

            List<RaceResultEntry> results = CurrentRace.FinishRace();

            CompletedRaces++;

            CurrentRace = null;

            return results;
        }

        public bool SaveCareer(out string error)
        {
            if(PlayerTeam == null || Teams == null || Teams.Count == 0)
            {
                error = "No active career";
                return false;
            }

            if(HasActiveRace)
            {
                error = "Finish the current race before saving the career";
                return false;
            }

            return CareerSaveSystem.TrySaveCareer(Teams, CompletedRaces, out error);
        }

        private Team CreatePlayerTeam()
        {
            Car car = new Car(
                "VRX-08",
                70,
                65,
                new Medium()
            );

            Driver driver = new Driver(
                "Morgan",
                70,
                65
            );

            return new Team(
                "VORTEX RACING",
                car,
                15000,
                Strategy.NORMAL,
                driver,
                Defence.NORMAL,
                isPlayerControlled : true
            );
        }

        private List<Team> CreateOpponentTeams()
        {
            return new List<Team>
            {
                new Team(
                    "APEX MOTORSPORT",
                    new Car("APX-01", 72, 68, new Hard()),
                    12000,
                    Strategy.NORMAL,
                    new Driver("Cole", 72, 60)),

                new Team(
                    "NOVA RACING",
                    new Car("NVR-02", 68, 72, new Medium()),
                    12000,
                    Strategy.NORMAL,
                    new Driver("Taylor", 69, 72)),

                new Team(
                    "TITAN GP",
                    new Car("TGN-03", 74, 62, new Soft()),
                    12000,
                    Strategy.NORMAL,
                    new Driver("Jordan", 71, 68))
            };
        }

        public bool LoadCareer(out string error)
        {
            if(HasActiveRace)
            {
                error = "Finish the current race before loading a career";
                return false;
            }

            if(!CareerSaveSystem.TryLoadCareer(
                out FullCareerSaveData saveData,
                out List<Team> loadedTeams,
                out error
            ))  
            {
                return false;
            }

            if(saveData == null || loadedTeams == null || loadedTeams.Count == 0)
            {
                error = "Loaded career data is invalid";
                return false;
            }

            if(loadedTeams.Exists(team => team == null))
            {
                error = "Loaded career contains an empty team";
                return false;
            }

            List<Team> playerTeams = loadedTeams
                .Where(team => team.IsPlayerControlled)
                .ToList();

            if(playerTeams.Count != 1)
            {
                error = "Save must contain exactly one player team";
                return false;
            }

            PlayerTeam = playerTeams[0];
            Teams = loadedTeams;
            CompletedRaces = saveData.CompletedRaces;
            CurrentRace = null;

            error = null;
            return true;
        }
    }
}