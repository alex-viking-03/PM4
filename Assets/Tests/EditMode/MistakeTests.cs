using NUnit.Framework;
using RacingProject.Domain;
using RacingProject.Simulation;
using System.Collections.Generic;

public class MistakeTests
{
    [Test]
    public void CalculateMistakeChance_MoreProfessionalDriver_ReducesChance()
    {
        Team professionalTeam = new Team(
            "Professional Team",
            new Car("Professional Car", 80, 90, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Professional", 100, 100),
            Defence.NORMAL
        );

        Team unprofessionalTeam = new Team(
            "Unprofessional Team",
            new Car("Unprofessional Car", 80, 90, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Unprofessional", 10, 100),
            Defence.NORMAL
        );

        Race race = new Race(
            new List<Team> {professionalTeam, unprofessionalTeam},
            new Track("Test Track", 4.5, 10, 3),
            new List<double> {1500, 1000}
        );

        double professionalChance = race.CalculateMistakeChance(professionalTeam);
        double unprofessionalChance = race.CalculateMistakeChance(unprofessionalTeam);

        Assert.Less(professionalChance, unprofessionalChance);
    }

    [Test]
    public void CalculateMistakeChance_WorseTireCondition_IncreasesChance()
    {
        Team wearedTiresTeam = new Team(
            "Weared tires Team",
            new Car("Weared tires Car", 80, 90, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Weared tires", 80, 90),
            Defence.NORMAL
        );

        wearedTiresTeam.Car.Tire.Wear(2);

        Team nonwearedTiresTeam = new Team(
            "Full tires Team",
            new Car("Full tires Car", 80, 90, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Full tires", 80, 90),
            Defence.NORMAL
        );

        Race race = new Race(
            new List<Team> {wearedTiresTeam, nonwearedTiresTeam},
            new Track("Test Track", 4.5, 10, 3),
            new List<double> {1500, 1000}
        );

        double wearedTiresChance = race.CalculateMistakeChance(wearedTiresTeam);
        double nonwearedTiresChance = race.CalculateMistakeChance(nonwearedTiresTeam);

        Assert.Greater(wearedTiresChance, nonwearedTiresChance);
    }

    [Test]
    public void CalculateMistakeChance_AttackStrategy_IncreasesChance()
    {
        Team attackTeam = new Team(
            "Attack Team",
            new Car("Attack Car", 80, 90, new Soft()),
            10000,
            Strategy.ATTACK,
            new Driver("Attack", 80, 90),
            Defence.NORMAL
        );

        Team normalTeam = new Team(
            "Normal Team",
            new Car("Normal Car", 80, 90, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Normal", 80, 90),
            Defence.NORMAL
        );

        Race race = new Race(
            new List<Team> {attackTeam, normalTeam},
            new Track("Test Track", 4.5, 10, 3),
            new List<double> {1500, 1000}
        );

        double attackChance = race.CalculateMistakeChance(attackTeam);
        double normalChance = race.CalculateMistakeChance(normalTeam);

        Assert.Greater(attackChance, normalChance);
    }

    [Test]
    public void CalculateMistakeChance_ConserveStrategy_ReducesChance()
    {
        Team conserveTeam = new Team(
            "Conserve Team",
            new Car("Conserve Car", 80, 90, new Soft()),
            10000,
            Strategy.CONSERVE,
            new Driver("Conserve", 80, 90),
            Defence.NORMAL
        );

        Team normalTeam = new Team(
            "Normal Team",
            new Car("Normal Car", 80, 90, new Soft()),
            10000,
            Strategy.NORMAL,
            new Driver("Normal", 80, 90),
            Defence.NORMAL
        );

        Race race = new Race(
            new List<Team> {conserveTeam, normalTeam},
            new Track("Test Track", 4.5, 10, 3),
            new List<double> {1500, 1000}
        );

        double conserveChance = race.CalculateMistakeChance(conserveTeam);
        double normalChance = race.CalculateMistakeChance(normalTeam);

        Assert.Less(conserveChance, normalChance);
    }

    [Test]
    public void CalculateMistakeChance_AggressiveDefence_IncreasesChance()
    {
        Team normalTeam = new Team(
            "Normal Team",
            new Car("Normal Car", 80, 90, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Normal Driver", 80, 50),
            Defence.NORMAL
        );

        Team aggressiveTeam = new Team(
            "Aggressive Team",
            new Car("Aggressive Car", 80, 90, new Hard()),
            10000,
            Strategy.NORMAL,
            new Driver("Aggressive Driver", 80, 50),
            Defence.AGGRESSIVE
        );

        Race race = new Race(
            new List<Team> { normalTeam, aggressiveTeam },
            new Track("Test Track", 4.5, 10, 3),
            new List<double> { 1500, 1000 }
        );

        double normalChance = race.CalculateMistakeChance(normalTeam);
        double aggressiveChance = race.CalculateMistakeChance(aggressiveTeam);

        Assert.Greater(aggressiveChance, normalChance);
    }
}