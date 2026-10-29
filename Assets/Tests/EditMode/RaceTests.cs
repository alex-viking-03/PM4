using System.Collections.Generic;
using NUnit.Framework;
using RacingProject.Domain;
using RacingProject.Simulation;

public class RaceTests
{
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
            new List<Team> {team},
            track,
            new List<double> {1500}
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
            new List<Team> {team},
            track,
            new List<double> {1500}
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

        Assert.AreEqual(98.0, tire.Condition);
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
            new List<Team> {team},
            track,
            new List<double> {1500}
        );

        race.StartRun();
        race.RunNextLap();

        Assert.AreEqual(98.0, team.Car.Tire.Condition);
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
            new List<Team> {team},
            track,
            new List<double> {1500}
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
            new List<Team> {team},
            track,
            new List<double> {1500},
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
    public void RunNextLap_WithWornCar_AppliesYieldDefence()
    {
        Car car = new Car("Test Car", 70, 60, new Hard());
        car.Wear(75);

        Driver driver = new Driver("Test Driver", 75, 90);

        Team team = new Team(
            "Test Team",
            car,
            10000,
            Strategy.ATTACK,
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
        race.RunNextLap();

        Assert.AreSame(Defence.YIELD, team.Defence);
    }

    [Test]
    public void CalculateMistakeChance_YieldHasLowerRiskThanAggressive()
    {
        Car car = new Car("Test Car", 70, 60, new Hard());
        Driver driver = new Driver("Test Driver", 75, 90);

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

        double aggressiveChance;
        double yieldChance;

        race.StartRun();

        team.SetDefence(Defence.AGGRESSIVE);
        aggressiveChance = race.CalculateMistakeChance(team);

         team.SetDefence(Defence.YIELD);
        yieldChance = race.CalculateMistakeChance(team);

        Assert.Greater(aggressiveChance, yieldChance);
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
    public void TryOvertake_WhenTeamsAreNotAdjacent_ReturnsFalse()
    {
        Car car1 = new Car("Car 1", 70, 60, new Hard());
        Car car2 = new Car("Car 2", 70, 60, new Hard());
        Car car3 = new Car("Car 3", 70, 60, new Hard());

        Team team1 = new Team("Team 1", car1, 10000, Strategy.NORMAL,
            new Driver("Driver 1"));

        Team team2 = new Team("Team 2", car2, 10000, Strategy.NORMAL,
            new Driver("Driver 2"));

        Team team3 = new Team("Team 3", car3, 10000, Strategy.NORMAL,
            new Driver("Driver 3"));

        Track track = new Track("Test Track", 4.5, 10, 3);

        Race race = new Race(
            new List<Team> { team1, team2, team3 },
            track,
            new List<double> { 1500, 1000, 500 },
            new NoMistakeRandom()
        );

        race.StartRun();

        bool overtook = race.TryOvertake(team3, team1);

        Assert.IsFalse(overtook);

        CollectionAssert.AreEqual(
            new[] { team1, team2, team3 },
            race.GetPositions()
        );
    }

    [Test]
    public void TryOvertake_WhenChanceIsOne_SwapsTeamPositions()
    {
        Car slowCar = new Car("Slow Car", 40, 40, new Hard());
        Car fastCar = new Car("Fast Car", 100, 100, new Hard());

        Driver slowDriver = new Driver("Slow Driver", 30, 20);
        Driver fastDriver = new Driver("Fast Driver", 100, 100);

        Team defender = new Team(
            "Defender",
            slowCar,
            10000,
            Strategy.NORMAL,
            slowDriver
        );

        Team attacker = new Team(
            "Attacker",
            fastCar,
            10000,
            Strategy.ATTACK,
            fastDriver
        );

        Track track = new Track("Test Track", 4.5, 10, 3);

        Race race = new Race(
            new List<Team> { defender, attacker },
            track,
            new List<double> { 1500, 1000 },
            new AlwaysMistakeRandom()
        );

        race.StartRun();

        bool overtook = race.TryOvertake(attacker, defender);

        Assert.IsTrue(overtook);

        CollectionAssert.AreEqual(
            new[] { attacker, defender },
            race.GetPositions()
        );
    }

    [Test]
    public void TryOvertake_WhenChanceIsZero_DoesNotSwapTeamPositions()
    {
        Car fastCar = new Car("Fast Car", 100, 100, new Hard());
        Car slowCar = new Car("Slow Car", 40, 40, new Hard());

        Driver fastDriver = new Driver("Fast Driver", 100, 100);
        Driver slowDriver = new Driver("Slow Driver", 30, 20);

        Team defender = new Team(
            "Defender",
            fastCar,
            10000,
            Strategy.NORMAL,
            fastDriver
        );

        Team attacker = new Team(
            "Attacker",
            slowCar,
            10000,
            Strategy.CONSERVE,
            slowDriver
        );

        Track track = new Track("Test Track", 4.5, 10, 3);

        Race race = new Race(
            new List<Team> { defender, attacker },
            track,
            new List<double> { 1500, 1000 },
            new AlwaysMistakeRandom()
        );

        race.StartRun();

        bool overtook = race.TryOvertake(attacker, defender);

        Assert.IsFalse(overtook);

        CollectionAssert.AreEqual(
            new[] { defender, attacker },
            race.GetPositions()
        );
    }
}
