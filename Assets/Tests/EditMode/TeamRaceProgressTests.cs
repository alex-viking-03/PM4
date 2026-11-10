using NUnit.Framework;
using RacingProject.Simulation;
using System;

public class TeamRaceProgressTests
{
    private TeamRaceProgress progress;

    [SetUp]
    public void SetUp()
    {
        progress = new TeamRaceProgress(5, 10);
    }

    [Test]
    public void Advance_ShouldIncreaseDistance()
    {
        progress.Advance(2.5, 30);

        Assert.AreEqual(2.5, progress.DistanceCovered);
    }

    [Test]
    public void CurrentLap_ShouldUpdateAfterCompletingLap()
    {
        progress.Advance(5, 60);

        Assert.AreEqual(1, progress.CompletedLaps);
        Assert.AreEqual(2, progress.CurrentLap);
    }

    [Test]
    public void IsFinished_ShouldBeTrueAfterCompletingRace()
    {
        progress.Advance(50, 600);

        Assert.IsTrue(progress.IsFinished);
        Assert.AreEqual(50, progress.DistanceCovered);
    }

    [Test]
    public void Advance_ShouldNotExceedTotalDistance()
    {
        progress.Advance(60, 600);

        Assert.AreEqual(50, progress.DistanceCovered);
        Assert.IsTrue(progress.IsFinished);
    }

    [Test]
    public void Constructor_ShouldThrow_WhenTrackLengthIsInvalid()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new TeamRaceProgress(0, 10)
        );
    }

    [Test]
    public void Constructor_ShouldThrow_WhenLapsAreInvalid()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new TeamRaceProgress(5, 0)
        );
    }
}
