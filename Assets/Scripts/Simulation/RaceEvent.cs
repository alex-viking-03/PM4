using RacingProject.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace RacingProject.Simulation
{
    public class RaceEvent
    {
        public Team Attacker {  get; }
        public Team Defender { get; }
        
        public string Description { get; }
        public RaceEventType Type { get; }
        public RaceEvent(
            RaceEventType type,
            Team attacker,
            Team defender,
            string description)
        {
            Type = type;
            Attacker = attacker;
            Defender = defender;
            Description = description;
        }


    }
}
