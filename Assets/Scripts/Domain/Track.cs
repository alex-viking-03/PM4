namespace RacingProject.Domain
{
    public class Track
    {
        public string Name {get;}
        public double Length {get;}
        public int Laps {get;}
        public double Difficulty {get;}

        public Track(
            string name,
            double length, //km
            int laps,
            double difficulty
        )
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException("Track name cannot be empty");
            }

            if (length <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(length));
            }

            if (laps <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(laps));
            }

            if (difficulty <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(difficulty));
            }

            Name = name;
            Length = length;
            Laps = laps;
            Difficulty = difficulty;
        }    

        public double GetTotalDistance()
        {
            return Length * Laps;
        }
    }
}