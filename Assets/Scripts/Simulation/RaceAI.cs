using RacingProject.Domain;
using System;

namespace RacingProject.Simulation
{
    public class RaceAI
    {
        public Defence ChooseDefence(Team team)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }

            double tireCondition = team.Car.Tire.Condition;
            double carCondition = team.Car.Condition;
            int aggressiveness = team.Driver.Aggressiveness;

            if (tireCondition < 25 || carCondition < 30)
            {
                return Defence.YIELD;
            }

            if (aggressiveness >= 70 &&
                tireCondition >= 60 &&
                carCondition >= 60)
            {
                return Defence.AGGRESSIVE;
            }

            return Defence.NORMAL;
        }

        public Strategy ChooseStrategy(Team team)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }

            double tireCondition = team.Car.Tire.Condition;
            double carCondition = team.Car.Condition;
            int aggressiveness = team.Driver.Aggressiveness;

            if (tireCondition < 25 || carCondition < 30)
            {
                return Strategy.CONSERVE;
            }

            if (aggressiveness >= 70 &&
                tireCondition >= 60 &&
                carCondition >= 60)
            {
                return Strategy.ATTACK;
            }

            return Strategy.NORMAL;
        }

        public Strategy ChooseStrategy(
            Team team,
            int position,
            int lapsRemaining,
            double gapToAhead
        )
        {
            if(team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }

            if(position < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            if(lapsRemaining < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lapsRemaining));
            }

            if(gapToAhead < 0 ||
               double.IsNaN(gapToAhead) ||
               double.IsInfinity(gapToAhead))
            {
                throw new ArgumentOutOfRangeException(nameof(gapToAhead));
            }

            double tireCondition = team.Car.Tire.Condition;
            double carCondition = team.Car.Condition;

            if(tireCondition < 25 || carCondition < 30)
            {
                return Strategy.CONSERVE;
            }

            if(lapsRemaining <= 3)
            {
                if(position == 1)
                {
                    return Strategy.CONSERVE;
                }
                
                if(gapToAhead <= 5)
                {
                    return Strategy.ATTACK;
                }

                return Strategy.NORMAL;
            } 

            return ChooseStrategy(team);
        }

        public bool CanCatchUp(
            double gapToAhead,
            double ownLapTime,
            double opponentLapTime,
            int lapsRemaining
        )
        {
            if(gapToAhead < 0 ||
               double.IsNaN(gapToAhead) ||
               double.IsInfinity(gapToAhead))
            {
                throw new ArgumentOutOfRangeException(nameof(gapToAhead));
            }

            if(ownLapTime <= 0 ||
               double.IsNaN(ownLapTime) ||
               double.IsInfinity(ownLapTime))
            {
                throw new ArgumentOutOfRangeException(nameof(ownLapTime));
            }

            if (opponentLapTime <= 0 ||
                double.IsNaN(opponentLapTime) ||
                double.IsInfinity(opponentLapTime))
            {
                throw new ArgumentOutOfRangeException(nameof(opponentLapTime));
            }

            if (lapsRemaining < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lapsRemaining));
            }

            if (gapToAhead == 0)
            {
                return true;
            }

            double timeAdvantagePerLap = opponentLapTime - ownLapTime;

            if (timeAdvantagePerLap <= 0)
            {
                return false;
            }

            double estimatedRecoveryTime =
                gapToAhead / timeAdvantagePerLap;

            return estimatedRecoveryTime <= lapsRemaining;
        }

        public Strategy ChooseStrategy(
            Team team,
            int position,
            int lapsRemaining,
            double gapToAhead,
            double ownLapTime,
            double opponentLapTime
        )
        {
            if(team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }

            if(position < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            if(lapsRemaining < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lapsRemaining));
            }

            double tireCondition = team.Car.Tire.Condition;
            double carCondition = team.Car.Condition;

            if(tireCondition < 25 || carCondition < 30)
            {
                return Strategy.CONSERVE;
            }

            if(lapsRemaining > 3)
            {
                return ChooseStrategy(team);
            }

            if(position == 1)
            {
                return Strategy.CONSERVE;
            }

            double attackLapTime = ownLapTime * Strategy.ATTACK.TimeMultiplier;

            bool canCatchUp = CanCatchUp(
                gapToAhead,
                attackLapTime,
                opponentLapTime,
                lapsRemaining
            );

            if(canCatchUp)
            {
                return Strategy.ATTACK;
            }

            return Strategy.NORMAL;
        }
    }
}

