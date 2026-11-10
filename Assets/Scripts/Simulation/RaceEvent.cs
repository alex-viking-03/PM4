using RacingProject.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace RacingProject.Simulation
{
    public class RaceEvent
    {
        public string Description { get; }
        public RaceEventType Type { get; }
        public RaceEvent(
            RaceEventType type,
            string description)
        {
            Type = type;
            Description = description;
        }
    }
}
