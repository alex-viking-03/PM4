using NUnit.Framework;
using RacingProject.Domain;
using RacingProject.Simulation;

public class RaceAITests
{
    [Test]
    public void ChooseStrategy_WithWearedTires_ReturnsCONSERVE()
    {
        Team team = new Team(
            "Weared tires team",
            new Car("Weared tires", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Weared tires", 65, 75),
            Defence.AGGRESSIVE
        );

        team.Car.Tire.Wear(70);

        RaceAI raceAI = new RaceAI();

        Assert.AreEqual(raceAI.ChooseStrategy(team), Strategy.CONSERVE);
    }

    [Test]
    public void ChooseStrategy_WithWearedCar_ReturnsCONSERVE()
    {
        Team team = new Team(
            "Weared car team",
            new Car("Weared car", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Weared car", 65, 75),
            Defence.AGGRESSIVE
        );

        team.Car.Wear(75);

        RaceAI raceAI = new RaceAI();

        Assert.AreEqual(raceAI.ChooseStrategy(team), Strategy.CONSERVE);
    }

    [Test]
    public void ChooseStrategy_WithAggressiveDriver_ReturnsATTACK()
    {
        Team team = new Team(
            "Aggressive driver team",
            new Car("Aggressive driver car", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Aggressive driver", 75, 90),
            Defence.AGGRESSIVE
        );

        RaceAI raceAI = new RaceAI();

        Assert.AreEqual(raceAI.ChooseStrategy(team), Strategy.ATTACK);
    }

    [Test]
    public void ChooseStrategy_WithAverageParameters_ReturnNORMAL()
    {
        Team team = new Team(
            "Usual team",
            new Car("Usual", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Usual", 65, 65),
            Defence.AGGRESSIVE
        );

        RaceAI raceAI = new RaceAI();

        Assert.AreEqual(raceAI.ChooseStrategy(team), Strategy.NORMAL);
    }

    [Test]
    public void ChooseStrategy_LastLapAndNotLeading_ReturnsATTACK()
    {
        Team team = new Team(
            "Chasing team",
            new Car("Car", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver", 65, 65)
        );

        RaceAI raceAI = new RaceAI();

        Assert.AreSame(
            Strategy.ATTACK,
            raceAI.ChooseStrategy(team, 2, 2, 3)
        );
    }

    [Test]
    public void ChooseStrategy_LastLapAndLeading_ReturnsCONSERVE()
    {
        Team team = new Team(
            "Leading Team",
            new Car("Car", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver", 65, 65)
        );

        RaceAI raceAI = new RaceAI();

        Assert.AreSame(
            Strategy.CONSERVE,
            raceAI.ChooseStrategy(team, 1, 2, 0)
        );
    }

    [Test]
    public void ChooseStrategy_LastLapWithWarnTires_ReturnsCONSERVE()
    {
        Team team = new Team(
            "Worn tires team",
            new Car("Car", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver", 90, 90)
        );

        team.Car.Tire.Wear(90);

        RaceAI raceAI = new RaceAI();

        Assert.AreSame(
            Strategy.CONSERVE,
            raceAI.ChooseStrategy(team, 2, 1, 2)
        );
    }

    [Test]
    public void ChooseStrategy_CloseToOppoentOnLastLaps_ReturnsATTACK()
    {
        Team team = new Team(
            "Chasing team",
            new Car("Car", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver", 65, 65)
        );

        RaceAI raceAI = new RaceAI();

        Assert.AreSame(
            Strategy.ATTACK,
            raceAI.ChooseStrategy(team, 2, 2, 0.2)
        );
    }

    [Test]
    public void ChooseStrategy_FarBehindOnLastLaps_ReturnsNORMAL()
    {
        Team team = new Team(
            "Chasing team",
            new Car("Car", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver", 65, 65)
        );

        RaceAI raceAI = new RaceAI();

        Assert.AreSame(
            Strategy.NORMAL,
            raceAI.ChooseStrategy(team, 2, 2, 30)
        );
    }
    
    [Test]
    public void CanCatchUp_WithEnoughLaps_ReturnsTrue()
    {
        RaceAI raceAI = new RaceAI();

        bool result = raceAI.CanCatchUp(
            gapToAhead: 3,
            ownLapTime: 60,
            opponentLapTime: 61,
            lapsRemaining: 5
        );

        Assert.IsTrue(result);
    }

    [Test]
    public void CanCatchUp_WithNotEnoughLaps_ReturnsFalse()
    {
        RaceAI raceAI = new RaceAI();

        bool result = raceAI.CanCatchUp(
            gapToAhead: 3,
            ownLapTime: 60,
            opponentLapTime: 61,
            lapsRemaining: 1
        );

        Assert.IsFalse(result);
    }

    [Test]
    public void CanCatchUp_WhenOpponentIsFaster_ReturnsFalse()
    {
        RaceAI raceAI = new RaceAI();

        bool result = raceAI.CanCatchUp(
            gapToAhead: 3,
            ownLapTime: 61,
            opponentLapTime: 60,
            lapsRemaining: 5
        );

        Assert.IsFalse(result);
    }

    [Test]
    public void ChooseStrategy_CanCatchUpOpponentOnLastLaps_ReturnsATTACK()
    {
        Team team = new Team(
             "Chasing team",
            new Car("Car", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver", 65, 65)
        );

        RaceAI raceAI = new RaceAI();

        Strategy result = raceAI.ChooseStrategy(
            team,
            position: 2,
            lapsRemaining: 3,
            gapToAhead: 2,
            ownLapTime: 60,
            opponentLapTime: 60.5
        );

        Assert.AreSame(Strategy.ATTACK, result);
    }

    [Test]
    public void ChooseStrategy_CannotCatchUpOpponentOnLastLaps_ReturnsNORMAL()
    {
        Team team = new Team(
            "Chasing team",
            new Car("Car", 9, 7, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Driver", 65, 65)
        );

        RaceAI raceAI = new RaceAI();

        Strategy result = raceAI.ChooseStrategy(
            team,
            position: 2,
            lapsRemaining: 1,
            gapToAhead: 10,
            ownLapTime: 60,
            opponentLapTime: 60.5
        );

        Assert.AreSame(Strategy.NORMAL, result);
    }
}
