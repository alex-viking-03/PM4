using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RacingProject.Career;
using RacingProject.SaveSystem;

public class CareerUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject CareerHubPanel;

    [Header("MainMenu")]
    [SerializeField] private Button ContinueCareerButton;

    [Header("CareerHub")]
    [SerializeField] private TMP_Text TeamNameText;
    [SerializeField] private TMP_Text BudgetText;
    [SerializeField] private TMP_Text CompletedRacesText;
    [SerializeField] private TMP_Text DriverNameText;
    [SerializeField] private TMP_Text DriverStatsText;
    [SerializeField] private TMP_Text CarNameText;
    [SerializeField] private TMP_Text CarStatsText;

    private CareerManager CareerManager;

    public void Awake()
    {
        CareerManager = new CareerManager();

        if(ContinueCareerButton != null)
        {
            ContinueCareerButton.interactable = CareerSaveSystem.HasSave();
        }

        if(MainMenuPanel != null)
        {
            MainMenuPanel.SetActive(true);
        }

        if(CareerHubPanel != null)
        {
            CareerHubPanel.SetActive(false);
        }
    }

    public void StartNewCareer()
    {
        CareerManager.StartNewCareer();

        if (CareerManager.SaveCareer(out string error))
        {
            Debug.Log("New career created and saved");
        }
        else
        {
            Debug.LogError($"Career created, but save failed: {error}");
        }

        ShowCareerHub();
    }

    public void ContinueCareer()
    {
        if(CareerManager.LoadCareer(out string error))
        {
            Debug.Log("Career loaded");
            ShowCareerHub();
        }
        else
        {
            Debug.LogError($"Couldn't load career: {error}");

            if(ContinueCareerButton != null)
            {
                ContinueCareerButton.interactable = CareerSaveSystem.HasSave();
            }
        }
    }

    public void ShowCareerHub()
    {
        RefreshCareerHub();

        MainMenuPanel.SetActive(false);
        CareerHubPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        CareerHubPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    private void RefreshCareerHub()
    {
        var team = CareerManager.PlayerTeam;

        if (team == null)
        {
            Debug.LogError("Player team was not created or loaded");
            return;
        }

        if (TeamNameText == null ||
            BudgetText == null ||
            CompletedRacesText == null ||
            DriverNameText == null ||
            DriverStatsText == null ||
            CarNameText == null ||
            CarStatsText == null)
        {
            Debug.LogError("CareerHub: не все текстовые поля назначены в Inspector!");
            return;
        }

        TeamNameText.text = team.Name;
        BudgetText.text = $"Бюджет: {team.Budget:N0}";
        CompletedRacesText.text = $"Завершено гонок: {CareerManager.CompletedRaces}";

        DriverNameText.text = $"Пилот: {team.Driver.Name}";
        DriverStatsText.text = $"Навык: {team.Driver.Skill}\nАгрессивность: {team.Driver.Aggressiveness}";

        CarNameText.text = $"Болид: {team.Car.Name}";
        CarStatsText.text =$"Скорость: {team.Car.Speed}\nУправляемость: {team.Car.Handling}";
    }
    
    public void Exit()
    {
        Debug.Log("Stopping game");
        Application.Quit();
    }
}
