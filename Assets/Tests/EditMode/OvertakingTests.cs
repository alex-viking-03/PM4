using System.Collections.Generic;
using NUnit.Framework;
using RacingProject.Domain;
using RacingProject.Simulation;
using System;

public class OvertakingTests
{
    private Race CreateRace(Team attackingTeam, Team defendingTeam)
    {
        return new Race(
            new List<Team> {attackingTeam, defendingTeam},
            new Track("Test Track", 4.5, 10, 3),
            new List<double> {1500, 1000}
        );
    }

    [Test]
    public void AttamptOvertaking_WhenAttackerIsMuchStronger_ReturnsSuccess()
    {
        Team attacker = new Team(
            "Fast Team",
            new Car("Fast Car", 100, 100, new Soft()),
            10000,
            Strategy.ATTACK,
            new Driver("Skilled Driver", 100, 100),
            Defence.NORMAL
        );

        Team defender = new Team(
            "Slow Team",
            new Car("Slow Car", 30, 20, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Weak Driver", 10, 10),
            Defence.YIELD
        );

        Race race = CreateRace(attacker, defender);

        string result = race.AttemptOvertaking(attacker, defender);

        Assert.AreEqual("success", result);
    }

    [Test]
    public void AttamptOvertaking_WhenDefenderIsMuchStronger_ReturnsFailed()
    {
        Team attacker = new Team(
            "Slow Team",
            new Car("Slow Car", 30, 20, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Weak Driver", 10, 10),
            Defence.YIELD
        );

        Team defender = new Team(
            "Fast Team",
            new Car("Fast Car", 100, 100, new Soft()),
            10000,
            Strategy.ATTACK,
            new Driver("Skilled Driver", 100, 100),
            Defence.NORMAL
        );

        Race race = CreateRace(attacker, defender);

        string result = race.AttemptOvertaking(attacker, defender);
        Assert.AreEqual("failed", result);
    }

    [Test]
    public void CalculateOvertakingChance_WithEqualScores_ReturnsHalf()
    {
        Team attacker = new Team(
            "Attacker",
            new Car("Attack Car", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Attacker", 70, 0),
            Defence.NORMAL
        );

        Team defender = new Team(
            "Defender",
            new Car("Defend Car", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Defender", 70, 0),
            Defence.NORMAL
        );

        Race race = CreateRace(attacker, defender);

        double chance = race.CalculateOvertakingChance(attacker, defender);

        Assert.AreEqual(0.5, chance, 0.0001);
    }

    [Test]
    public void CalculateOvertakingChance_WithStrongAttacker_ReturnsOne()
    {
        Team attacker = new Team(
            "Attacker",
            new Car("Attack Car", 100, 100, new Soft()),
            10000,
            Strategy.ATTACK,
            new Driver("Attacker", 100, 100),
            Defence.NORMAL
        );

        Team defender = new Team(
            "Defender",
            new Car("Defend Car", 30, 20, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Defender", 10, 10),
            Defence.YIELD
        );

        Race race = CreateRace(attacker, defender);

        double chance = race.CalculateOvertakingChance(attacker, defender);

        Assert.AreEqual(1.0, chance, 0.0001);
    }

    [Test]
    public void CalculateOvertakingChance_WithStrongDefender_ReturnsZero()
    {
        Team defender = new Team(
            "Defender",
            new Car("Defend Car", 100, 100, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Defender", 100, 100),
            Defence.AGGRESSIVE
        );

        Team attacker = new Team(
            "Attacker",
            new Car("Attack Car", 30, 20, new Hard()),
            10000,
            Strategy.CONSERVE,
            new Driver("Attacker", 10, 10),
            Defence.YIELD
        );

        Race race = CreateRace(attacker, defender);

        double chance = race.CalculateOvertakingChance(attacker, defender);

        Assert.AreEqual(0.0, chance, 0.0001);
    }

    [Test]
    public void CalculateOvertakingChance_AggresiveDefence_ReducesChance()
    {
        Team attacker = new Team(
            "Attacker",
            new Car("Attacker car", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Attacker", 70, 50),
            Defence.NORMAL
        );

        Team normalDefender = new Team(
            "Normal Defender",
            new Car("Normal Defender car", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Normal Defender", 70, 50),
            Defence.NORMAL
        );

        Team aggressiveDefender = new Team(
            "Aggressive Defender",
            new Car("Aggressive Defender car", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Aggressive Defender", 70, 50),
            Defence.AGGRESSIVE
        );

        Race normalRace = CreateRace(attacker, normalDefender);
        Race aggressiveRace = CreateRace(attacker, aggressiveDefender);

        double normalChance = normalRace.CalculateOvertakingChance(attacker, normalDefender);
        double aggressiveChance = aggressiveRace.CalculateOvertakingChance(attacker, aggressiveDefender);

        Assert.Less(aggressiveChance, normalChance);
    }

    [Test]
    public void CalculateOvertakingChance_YIELDDefence_IncreasesChance()
    {
        Team attacker = new Team(
            "Attacker",
            new Car("Attacker car", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Attacker", 70, 50),
            Defence.NORMAL
        );

        Team normalDefender = new Team(
            "Normal Defender",
            new Car("Normal Defender car", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Normal Defender", 70, 50),
            Defence.NORMAL
        );

        Team yieldDefender = new Team(
            "Yield Defender",
            new Car("Yield Defender car", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Yield Defender", 70, 50),
            Defence.YIELD
        );

        Race normalRace = CreateRace(attacker, normalDefender);
        Race yieldRace = CreateRace(attacker, yieldDefender);

        double normalChance = normalRace.CalculateOvertakingChance(attacker, normalDefender);
        double yieldChance = yieldRace.CalculateOvertakingChance(attacker, yieldDefender);

        Assert.Greater(yieldChance, normalChance);
    }

    [Test]
    public void CalculateOvertakingChance_AttackStrategy_IncreasesChance()
    {
        Team normalAttacker = new Team(
            "Normal Attacker",
            new Car("Normal Attacker car", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Normal Attacker", 70, 50),
            Defence.NORMAL
        );

        Team aggressiveAttacker = new Team(
            "Aggressive Attacker",
            new Car("Aggressive Attacker car", 70, 70, new Hard()),
            10000,
            Strategy.ATTACK,
            new Driver("Aggressive Attacker", 70, 50),
            Defence.NORMAL
        );

        Team defender = new Team(
            "Defender",
            new Car("Defender car", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Defender", 70, 50),
            Defence.NORMAL
        );

        Race normalRace = CreateRace(normalAttacker, defender);
        Race attackRace = CreateRace(aggressiveAttacker, defender);

        double normalChance = normalRace.CalculateOvertakingChance(normalAttacker, defender);
        double attackChance = attackRace.CalculateOvertakingChance(aggressiveAttacker, defender);

        Assert.Greater(attackChance, normalChance);
    }

    [Test]
    public void StartRun_InitializesPositionsInTeamOrder()
    {
        Team first = new Team(
            "First Team",
            new Car("Car A", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver A")
        );

        Team second = new Team(
            "Second Team",
            new Car("Car B", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver B")
        );

        Race race = CreateRace(first, second);
        race.StartRun();

        List<Team> positions = race.GetPositions();

        Assert.AreEqual(2, positions.Count);
        Assert.AreSame(first, positions[0]);
        Assert.AreSame(second, positions[1]);
    }

    [Test]
    public void GetPositions_ReturnsCopyOfPositions()
    {
        Team first = new Team(
            "First Team",
            new Car("Car A", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver A")
        );

        Team second = new Team(
            "Second Team",
            new Car("Car B", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver B")
        );

        Race race = CreateRace(first, second);
        race.StartRun();

        List<Team> positions = race.GetPositions();
        positions.Reverse();

        List<Team> actualPositions = race.GetPositions();

        Assert.AreSame(first, actualPositions[0]);
        Assert.AreSame(second, actualPositions[1]);
    }

    [Test]
    public void TryOvertake_WhenSuccessful_SwapsPositions()
    {
        Team defender = new Team(
            "Slow Team",
            new Car("Slow Car", 30, 20, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Weak Driver", 10, 10),
            Defence.YIELD
        );

        Team attacker = new Team(
            "Fast Team",
            new Car("Fast Car", 100, 100, new Soft()),
            10000,
            Strategy.ATTACK,
            new Driver("Skilled Driver", 100, 100),
            Defence.NORMAL
        );

        Race race = CreateRace(defender, attacker);
        race.StartRun();

        bool result = race.TryOvertake(attacker, defender);

        Assert.IsTrue(result);

        List<Team> positions = race.GetPositions();

        Assert.AreSame(attacker, positions[0]);
        Assert.AreSame(defender, positions[1]);
    }

    [Test]
    public void TryOvertake_WhenAttackerIsAhead_ReturnsFalse()
    {
        Team first = new Team(
            "First Team",
            new Car("Car A", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver A")
        );

        Team second = new Team(
            "Second Team",
            new Car("Car B", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver B")
        );

        Race race = CreateRace(first, second);
        race.StartRun();

        bool result = race.TryOvertake(first, second);

        Assert.IsFalse(result);
    }

    [Test]
    public void TryOvertake_WhenAttackerIsNotAdjacent_ReturnsFalse()
    {
        Team first = new Team(
            "First Team",
            new Car("Car A", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver A")
        );

        Team second = new Team(
            "Second Team",
            new Car("Car B", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver B")
        );

        Team third = new Team(
            "Third Team",
            new Car("Car C", 70, 70, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver C")
        );

        Race race = new Race(
            new List<Team> { first, second, third },
            new Track("Test Track", 4.5, 10, 3),
            new List<double> { 1500, 1000, 500 }
        );

        race.StartRun();

        bool result = race.TryOvertake(third, first);

        Assert.IsFalse(result);
    }

    [Test]
    public void RunNextLap_WhenAttackerIsFaster_CanOvertakeAutomatically()
    {
        Team defender = new Team(
            "Slow Team",
            new Car("Slow Car", 30, 20, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Weak Driver", 10, 10),
            Defence.YIELD
        );

        Team attacker = new Team(
            "Fast Team",
            new Car("Fast Car", 100, 100, new Soft()),
            10000,
            Strategy.ATTACK,
            new Driver("Skilled Driver", 100, 100),
            Defence.NORMAL
        );

        Race race = CreateRace(defender, attacker);

        race.StartRun();
        race.RunNextLap();

        List<Team> positions = race.GetPositions();

        Assert.AreSame(attacker, positions[0]);
        Assert.AreSame(defender, positions[1]);
    }

    [Test]
    public void RunNextLap_WhenDriverMakesMistake_AddsLostTime()
    {
        Team team = new Team(
            "Risky Team",
            new Car("Test Car", 70, 60, new Hard()),
            10000,
            Strategy.ATTACK,
            new Driver("Risky Driver", 0, 100)
        );

        Race race = new Race(
            new List<Team> {team},
            new Track("Test Track", 4.5, 10, 3),
            new List<double> {1500},
            new FixedRandom(0.0)
        );

        race.StartRun();

        double expectedTime = race.CalculateTime(team.Car) * team.Strategy.TimeMultiplier;

        race.RunNextLap();

        double actualTime = race.GetResult(team).Time;

        Assert.Greater(actualTime, expectedTime);
    }

    [Test]
    public void SetDefence_ChangesTeamDefence()
    {
        Team team = new Team(
            "Test Team",
            new Car("Test Car", 70, 60, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Test Driver"),
            Defence.NORMAL
        );

        team.SetDefence(Defence.AGGRESSIVE);

        Assert.AreSame(Defence.AGGRESSIVE, team.Defence);
    }

    [Test]
    public void SetDefence_WithNull_ThrowsArgumentNullException()
    {
        Team team = new Team(
            "Test Team",
            new Car("Test Car", 70, 60, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Test Driver"),
            Defence.NORMAL
        );

        Assert.Throws<System.ArgumentNullException>(
            () => team.SetDefence(null)
        );
    }
}

public class FixedRandom: System.Random
{
    private readonly double value;

    public FixedRandom(double value)
    {
        this.value = value;
    }

    public override double NextDouble()
    {
        return value;
    }
}