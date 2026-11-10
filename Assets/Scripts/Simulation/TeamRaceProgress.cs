using System;
using System.Collections.Generic;
using System.Text;

namespace RacingProject.Simulation
{
    public class TeamRaceProgress
    {
        public double DistanceCovered {  get; private set; }
        public double TrackLength { get; }
        public double TotalDistance { get; }

        public int CompletedLaps =>
            (int)(DistanceCovered / TrackLength);

        public int CurrentLap =>
            Math.Min(CompletedLaps + 1, TotalLaps);

        public int TotalLaps { get; }

        public double DistanceOnCurrentLap =>
            DistanceCovered - CompletedLaps * TrackLength;

        public bool IsFinished =>
            DistanceCovered >= TotalDistance;

        public TeamRaceProgress(double trackLength, int totalLaps, double initialDistance = 0)
        {
            if(trackLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(trackLength));
            }
            if (totalLaps <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalLaps));
            }
            if(initialDistance < 0 || initialDistance > trackLength * totalLaps)
            {
                throw new ArgumentOutOfRangeException(nameof(initialDistance));
            }

            TrackLength = trackLength;
            TotalLaps = totalLaps;
            TotalDistance = trackLength * totalLaps;

            DistanceCovered = initialDistance;
        }

        public void Advance(double distance, double elapsedTime)
        {
            if (distance <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(distance));
            }

            if (elapsedTime <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedTime));
            }

            if (IsFinished)
            {
                return;
            }

            double remainingDistance = TotalDistance - DistanceCovered;

            double actualDistance = Math.Min(distance, remainingDistance);

            DistanceCovered += actualDistance;
        }
    }
}
