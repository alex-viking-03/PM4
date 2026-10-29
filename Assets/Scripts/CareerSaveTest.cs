
using UnityEngine;
using RacingProject.Career;
using System.IO;

public class CareerSaveTest : MonoBehaviour
{
    [ContextMenu("Run Save/Load Test")]
    private void RunSaveLoadTest()
    {
        CareerManager manager = new CareerManager();

        manager.StartNewCareer();

        double originalBudget = manager.PlayerTeam.Budget;
        double originalCarCondition = manager.PlayerTeam.Car.Condition;
        string originalTeamName = manager.PlayerTeam.Name;

        manager.PlayerTeam.AddMoney(5000);
        manager.PlayerTeam.Car.Wear(20);

        double expectedBudget = manager.PlayerTeam.Budget;
        double expectedCondition = manager.PlayerTeam.Car.Condition;

        if (!manager.SaveCareer(out string saveError))
        {
            Debug.LogError($"SAVE FAILED: {saveError}");
            return;
        }

        CareerManager loadedManager = new CareerManager();

        if (!loadedManager.LoadCareer(out string loadError))
        {
            Debug.LogError($"LOAD FAILED: {loadError}");
            return;
        }

        bool budgetCorrect =
            loadedManager.PlayerTeam.Budget == expectedBudget;

        bool conditionCorrect =
            loadedManager.PlayerTeam.Car.Condition == expectedCondition;

        bool nameCorrect =
            loadedManager.PlayerTeam.Name == originalTeamName;

        if (budgetCorrect && conditionCorrect && nameCorrect)
        {
            Debug.Log("SAVE/LOAD TEST PASSED");
        }
        else
        {
            Debug.LogError(
                $"SAVE/LOAD TEST FAILED\n" +
                $"Budget: expected {expectedBudget}, got {loadedManager.PlayerTeam.Budget}\n" +
                $"Car condition: expected {expectedCondition}, got {loadedManager.PlayerTeam.Car.Condition}\n" +
                $"Team name: expected {originalTeamName}, got {loadedManager.PlayerTeam.Name}"
            );
        }
    }
}