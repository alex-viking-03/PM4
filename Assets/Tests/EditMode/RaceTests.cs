using NUnit.Framework;
using RacingProject.Domain;
using RacingProject.Simulation;
using System;
using System.Collections.Generic;

public class RaceTests
{
    private Team Team1;
    private Team Team2;
    private Track Track;
    private List<Team> Teams;
    private Race Race;
    private class NoMistakeRandom : System.Random
    {
        public override double NextDouble()
        {
            return 0.99;
        }
    }

    private class AlwaysMistakeRandom : System.Random
    {
        public override double NextDouble()
        {
            return 0.0;
        }
    }

    [SetUp]
    public void SetUp()
    {
        Team1 = new Team(
                    "team1",
                    new Car("car1", 100, 100, new Soft()),
                    15000,
                    Strategy.NORMAL,
                    new Driver("driver1"));

        Team2 = new Team(
                    "team2",
                    new Car("car2", 65, 70, new Soft()),
                    15000,
                    Strategy.NORMAL,
                    new Driver("driver2"));

        Teams = new List<Team> { Team1, Team2 };

        Track = new Track("test track", 5, 1, 5);
        Race = new Race(
            Teams,
            Track,
            new List<double> { 10000, 5000 });
    }

    [Test]
    public void TestExample()
    {
        Assert.AreEqual(2, 1 + 1);
    }

    [Test]
    public void CalculateTime_WithKnownValues_ReturnsExpectedTime()
    {
        Car car = new Car("Test Car", 70, 60, new Hard());
        Track track = new Track("Test Track", 4.5, 1, 3);

        Team team = new Team("Test Team", car, 10000, Strategy.ATTACK, new Driver("Test Driver"));

        Race race = new Race(
            new List<Team> { team },
            track,
            new List<double> { 1500 }
        );

        double actual = race.CalculateTime(car);

        double expected = 4.5 / 70 * 3600 * 1.12;

        Assert.AreEqual(expected, actual, 0.0001);
    }

    [Test]
    public void SoftTireWear_ReducesConditionByFour()
    {
        Tire tire = new Soft();

        tire.Wear();

        Assert.AreEqual(96, tire.Condition);
    }

    [Test]
    public void RepairCar_WithoutEnoughMoney_ReturnsFalse()
    {
        Car car = new Car("Test Car", 70, 60, new Hard());
        car.Wear(50);

        Team team = new Team("Test Team", car, 100);

        bool required = team.RepairCar();

        Assert.IsFalse(required);
        Assert.AreEqual(50, car.Condition);
        Assert.AreEqual(100, team.Budget);
    }

    [Test]
    public void CalculateTime_WithWornCarAndSoftTires_ReturnsExpectedTime()
    {
        Car car = new Car("Test Car", 70, 60, new Soft());
        car.Wear(20);
        car.Tire.Wear();

        Track track = new Track("Test Track", 4.5, 1, 3);
        Team team = new Team("Test Team", car, 10000, Strategy.ATTACK, new Driver("Test Driver"));

        Race race = new Race(
            new List<Team> { team },
            track,
            new List<double> { 1500 }
        );

        double actual = race.CalculateTime(car);

        double baseTime = 4.5 / 70 * 3600;
        double difficulty = 1 + (3.0 / 10) * ((100 - 60.0) / 100);
        double condition = 1 + (100 - 80.0) / 100;
        double tire = 1 / (1 + 0.08 * 96 / 100.0);

        double expected = baseTime * difficulty * condition * tire;

        Assert.AreEqual(expected, actual, 0.0001);
    }

    [Test]
    public void RunNextLap_WhenTeamLeadsOnFinalLap_AppliesConserveStrategy()
    {
        Car car = new Car("Test car", 70, 60, new Hard());

        Driver driver = new Driver("Test Driver");

        Team team = new Team(
            "Test team",
            car,
            10000,
            Strategy.ATTACK,
            driver
        );

        Track track = new Track("Test track", 4.5, 1, 3);

        Race race = new Race(
            new List<Team> { team },
            track,
            new List<double> { 1500 }
        );

        race.StartRun();
        race.RunNextLap();

        Assert.AreSame(Strategy.CONSERVE, team.Strategy);
    }

    [Test]
    public void SoftTireWear_WithConserveStrategy_ReducesWear()
    {
        Tire tire = new Soft();

        tire.Wear(Strategy.CONSERVE.WearMultiplier);

        Assert.AreEqual(97.6, tire.Condition);
    }

    [Test]
    public void SoftTireWear_WithAttackStrategy_IcreasesWear()
    {
        Tire tire = new Soft();

        tire.Wear(Strategy.ATTACK.WearMultiplier);

        Assert.AreEqual(94.0, tire.Condition);
    }

    [Test]
    public void RunNextLap_WithConserveStrategy_ReducesWear()
    {
        Car car = new Car("Test Car", 70, 60, new Soft());
        Driver driver = new Driver("Test Driver");

        Team team = new Team(
            "Test Team",
            car,
            10000,
            Strategy.CONSERVE,
            driver
        );

        Track track = new Track("Test track", 4.5, 2, 3);

        Race race = new Race(
            new List<Team> { team },
            track,
            new List<double> { 1500 }
        );

        race.StartRun();
        race.RunNextLap();

        Assert.AreEqual(97.6, team.Car.Tire.Condition);
    }

    [Test]
    public void RunNextLap_WithAttackStrategy_IncreasesWear()
    {
        Car car = new Car("Test Car", 70, 60, new Soft());
        Driver driver = new Driver("Test Driver", 75, 90);

        Team team = new Team(
            "Test Team",
            car,
            10000,
            Strategy.ATTACK,
            driver
        );

        Track track = new Track("Test track", 4.5, 10, 3);

        Race race = new Race(
            new List<Team> { team },
            track,
            new List<double> { 1500 }
        );

        race.StartRun();
        race.RunNextLap();

        Assert.AreEqual(94.0, team.Car.Tire.Condition);
    }

    [Test]
    public void RunNextLap_WithAttackStrategy_UsesTimeMultiplier()
    {
        Car car = new Car("Test Car", 70, 60, new Soft());
        Driver driver = new Driver("Test Driver", 75, 90);

        Team team = new Team(
            "Test Team",
            car,
            10000,
            Strategy.ATTACK,
            driver
        );

        Track track = new Track("Test track", 4.5, 10, 3);

        Race race = new Race(
            new List<Team> { team },
            track,
            new List<double> { 1500 },
            new NoMistakeRandom()
        );

        double baseTime = race.CalculateTime(car);

        race.StartRun();
        race.RunNextLap();

        RaceResult results = race.GetResult(team);

        Assert.AreSame(Strategy.ATTACK, team.Strategy);
        Assert.AreEqual(
            baseTime * Strategy.ATTACK.TimeMultiplier,
            results.Time,
            0.0001
        );
    }

    [Test]
    public void AttemptMistake_WhenRandomValueIsZero_RecordsLostTime()
    {
        Car car = new Car("Test Car", 70, 60, new Hard());
        Driver driver = new Driver("Test Driver", 75, 50);

        Team team = new Team(
            "Test Team",
            car,
            10000,
            Strategy.NORMAL,
            driver
        );

        Track track = new Track("Test Track", 4.5, 10, 3);

        Race race = new Race(
            new List<Team> { team },
            track,
            new List<double> { 1500 },
            new AlwaysMistakeRandom()
        );

        race.StartRun();

        double timeBefore = race.GetResult(team).Time;

        bool madeMistake = race.AttemptMistake(team);

        double timeAfter = race.GetResult(team).Time;

        Assert.IsTrue(madeMistake);
        Assert.Greater(timeAfter, timeBefore);
    }

    [Test]
    public void AttemptMistake_WhenRandomValueIsHigh_DoesNotRecordLostTime()
    {
        Car car = new Car("Test Car", 70, 60, new Hard());
        Driver driver = new Driver("Test Driver", 75, 50);

        Team team = new Team(
            "Test Team",
            car,
            10000,
            Strategy.NORMAL,
            driver
        );

        Track track = new Track("Test Track", 4.5, 10, 3);

        Race race = new Race(
            new List<Team> { team },
            track,
            new List<double> { 1500 },
            new NoMistakeRandom()
        );

        race.StartRun();

        double timeBefore = race.GetResult(team).Time;

        bool madeMistake = race.AttemptMistake(team);

        double timeAfter = race.GetResult(team).Time;

        Assert.IsFalse(madeMistake);
        Assert.AreEqual(timeBefore, timeAfter, 0.0001);
    }

    [Test]
    public void StartRun_ShouldInitializeProgressForEveryTeam()
    {
        Race.StartRun();

        foreach (Team team in Teams)
        {
            TeamRaceProgress progress = Race.GetTeamProgress(team);

            Assert.AreEqual(progress.DistanceCovered, 0);
            Assert.IsFalse(progress.IsFinished);
        }
    }

    [Test]
    public void CalculateDistance_ShouldCalculateDistanceForOneSecond()
    {
        double lapTime = Race.CalculateTime(Team1.Car) * Team1.Strategy.TimeMultiplier;

        double expectedDistance = Track.Length / lapTime;

        double actualDistance = Race.CalculateDistance(Team1, 1);

        Assert.AreEqual(expectedDistance, actualDistance, 0.0001);
    }

    [Test]
    public void AdvanceSimulationStep_ShouldIncreaseTeamProgress()
    {
        Race.StartRun();

        Race.AdvanceSimulationStep(1);

        TeamRaceProgress progress = Race.GetTeamProgress(Team1);

        Assert.Greater(progress.DistanceCovered, 0);
    }

    [Test]
    public void AdvanceSimulationStep_ShouldPlaceFasterTeamAhead()
    {
        Race.StartRun();

        Team fasterTeam = Team1;
        Team slowerTeam = Team2;

        double fasterLap = Race.CalculateTime(fasterTeam.Car) * fasterTeam.Strategy.TimeMultiplier;
        double slowerLap = Race.CalculateTime(slowerTeam.Car) * slowerTeam.Strategy.TimeMultiplier;

        Assert.Less(fasterLap, slowerLap);

        Race.AdvanceSimulationStep(1.0);

        List<Team> positions = Race.GetPositions();

        Assert.AreEqual(fasterTeam, positions[0]);
    }

    [Test]
    public void AdvanceSimulationStep_ShouldProcessMutlipleTimeSteps()
    {
        Race.StartRun();

        Race.AdvanceSimulationStep(1.0);

        foreach (Team team in Teams)
        {
            TeamRaceProgress progress = Race.GetTeamProgress(team);
        }
    }

    [Test]
    public void AdvanceSimulationStep_WhenTeamEntersOvertakingRange_AttemptsOvertaking()
    {
        Race.StartRun();
        Race.AdvanceSimulationStep(1.0);

        Assert.Greater(Race.GetTeamProgress(Team1).DistanceCovered, 0);
    }

    [Test]
    public void AdvanceSimulationStep_WhenAttackerEntersOvertakingRange_FailsOvertake()
    {
        Team defender = new Team(
            "Defender",
            new Car("Slow car", 40, 100, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Slow driver", 30, 20));

        Team attacker = new Team(
            "Attacker",
            new Car("Fast Car", 100, 40, new Hard()),
            10000,
            Strategy.CONSERVE,
            new Driver("Fast Driver", 100, 100));

        Track track = new Track("Test track", 5, 1, 5);

        Race race = new Race(
            new List<Team> { defender, attacker },
            track,
            new List<double> { 15000, 10000 },
            new NoMistakeRandom(),
            new List<double> { 1.0, 0.4995 });

        race.StartRun();

        race.AdvanceSimulationStep(0.1);

        CollectionAssert.AreEqual(new[] { defender, attacker },
            race.GetPositions());
    }

    [Test]
    public void TryOvertake_ShouldReturnFalse_WhenAttackerIsFinished()
    {
        Race newRace = new Race(
            Teams,
            Track,
            new List<double> { 10000, 5000 },
            startingDistances: new List<double> { Track.GetTotalDistance(), 0 });

        newRace.StartRun();

        List<Team> positionsBefore = newRace.GetPositions();

        CollectionAssert.AreEqual(
            positionsBefore,
            newRace.GetPositions()
        );
    }

    [Test]
    public void TryOvertake_ShouldReturnFalse_WhenDefenderIsFinished()
    {
        Race newRace = new Race(
            Teams,
            Track,
            new List<double> { 10000, 5000 },
            startingDistances: new List<double> { 0, Track.GetTotalDistance() });

        newRace.StartRun();

        List<Team> positionsBefore = newRace.GetPositions();

        CollectionAssert.AreEqual(
            positionsBefore,
            newRace.GetPositions()
        );
    }

    [Test]
    public void AdvanceSimulationStep_ShouldPreservePitStopTime()
    {
        Race.StartRun();

        Race.PitStop(Team1, new Medium());

        Race.AdvanceSimulationStep(1.0);

        RaceResult result = Race.GetResult(Team1);

        Assert.AreEqual(21.0, result.Time, 0.0001);
    }

    [Test]
    public void WhenFinishedTeamPerformsPitStop_ThrowsInvalidOperation()
    {
        Race.StartRun();

        while (!Race.GetTeamProgress(Team1).IsFinished)
        {
            Race.AdvanceSimulationStep(1.0);
        }

        Assert.Throws<InvalidOperationException>(
            () => Race.PitStop(Team1, new Medium()));
    }
}