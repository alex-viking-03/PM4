using UnityEngine;
using RacingProject.Domain;
using RacingProject.SaveSystem;

public class SaveSystemQuickTest : MonoBehaviour
{
    private string GetStrategyName(Strategy strategy)
    {
        if (ReferenceEquals(strategy, Strategy.ATTACK))
            return "ATTACK";

        if (ReferenceEquals(strategy, Strategy.CONSERVE))
            return "CONSERVE";

        return "NORMAL";
    }
}