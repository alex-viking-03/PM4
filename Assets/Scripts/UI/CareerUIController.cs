using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RacingProject.Career;
using RacingProject.SaveSystem;
using NUnit.Framework;

public class CareerUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject CareerHubPanel;
    [SerializeField] private GameObject RacePreparationPanel;
    [SerializeField] private GameObject RacePanel;

    [Header("Main menu")]
    [SerializeField] private Button ContinueCareerButton;

    [Header("Upgrade car")]
    [SerializeField] private Button UpgradeSpeedButton;
    [SerializeField] private Button UpgradeHandlingButton;

    [Header("Upgrade driver")]
    [SerializeField] private Button UpgradeDriverSkillButton;
    [SerializeField] private Button UpgradeDriverAggressivenessButton;

    [Header("Career hub")]
    [SerializeField] private TMP_Text TeamNameText;
    [SerializeField] private TMP_Text BudgetText;
    [SerializeField] private TMP_Text CompletedRacesText;
    [SerializeField] private TMP_Text ExpPointsText;
    [SerializeField] private TMP_Text DriverNameText;
    [SerializeField] private TMP_Text DriverStatsText;
    [SerializeField] private TMP_Text CarNameText;
    [SerializeField] private TMP_Text CarStatsText;

    [Header("Controllers")]
    [SerializeField] private RacePreparationController racePreparationController;

    private CareerManager CareerManager;

    public void Awake()
    {
        CareerManager = new CareerManager();

        if(ContinueCareerButton != null)
        {
            ContinueCareerButton.interactable = CareerSaveSystem.HasSave();
        }

        if (UpgradeSpeedButton != null)
        {
            UpgradeSpeedButton.onClick.AddListener(BuySpeedUpgrade);
        }

        if (UpgradeHandlingButton != null)
        {
            UpgradeHandlingButton.onClick.AddListener(BuyHandlingUpgrade);
        }

        if (UpgradeDriverSkillButton != null)
        {
            UpgradeDriverSkillButton.onClick.AddListener(BuyDriverSkillUpgrade);
        }

        if (UpgradeDriverAggressivenessButton != null)
        {
            UpgradeDriverAggressivenessButton.onClick.AddListener(BuyDriverAggressivenessUpgrade);
        }

        if (MainMenuPanel != null)
        {
            MainMenuPanel.SetActive(true);
        }

        if(CareerHubPanel != null)
        {
            CareerHubPanel.SetActive(false);
        }

        if(RacePreparationPanel != null)
        {
            RacePreparationPanel.SetActive(false);
        }

        if(RacePanel != null)
        {
            RacePanel.SetActive(false);
        }

        racePreparationController.Initialize(CareerManager);
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

        CarNameText.text = $"Болид: {team.Car.Name}";

        TeamNameText.text = team.Name;
        BudgetText.text = $"Бюджет: {team.Budget:N0}";
        CompletedRacesText.text = $"Завершено гонок: {CareerManager.CompletedRaces}";
        ExpPointsText.text = $"Количество улучшений гонщика: {team.Driver.DevelopmentPoints}";

        DriverNameText.text = $"Пилот: {team.Driver.Name}";
        DriverStatsText.text = $"Навык: {team.Driver.Skill}\nАгрессивность: {team.Driver.Aggressiveness}";

        RefershCarUpgrade();

        UpgradeDriverSkillButton.interactable = team.Driver.CanUpgradeSkill;

        UpgradeDriverAggressivenessButton.interactable = team.Driver.CanUpgradeAggressiveness;
    }
    
    public void Exit()
    {
        Debug.Log("Stopping game");
        Application.Quit();
    }

    public void RacePreparation()
    {
        racePreparationController.ShowRacePreparation();
    }

    private void RefershCarUpgrade()
    {
        var team = CareerManager.PlayerTeam;
        var car = team.Car;

        CarStatsText.text = $"Скорость: {car.Speed}, уровень: {car.SpeedLevel}/3\nУправляемость: {car.Handling}, уровень: {car.HandlingLevel}/3";

        bool canUpgradeSpeed = car.SpeedLevel < 3 &&
                           team.Budget >= team.NextSpeedUpgradeCost &&
                           car.Speed + team.NextSpeedUpgradeAmount <= 100;

        bool canUpgradeHandling = car.HandlingLevel < 3 &&
                                  team.Budget >= team.NextHandlingUpgradeCost &&
                                  car.Handling + team.NextHandlingUpgradeAmount <= 100;
        UpgradeSpeedButton.interactable = canUpgradeSpeed;
        UpgradeHandlingButton.interactable = canUpgradeHandling;
    }

    private void BuySpeedUpgrade()
    {
        var team = CareerManager.PlayerTeam;

        if (!team.BuySpeedUpgrade())
        {
            Debug.Log("Не удалось купить улучшение скорости.");
            RefreshCareerHub();
            return;
        }

        RefreshCareerHub();

        if (!CareerManager.SaveCareer(out string error))
        {
            Debug.LogError($"Улучшение куплено, но сохранение не удалось: {error}");
        }
    }

    private void BuyHandlingUpgrade()
    {
        var team = CareerManager.PlayerTeam;

        if (!team.BuyHandlingUpgrade())
        {
            Debug.Log("Не удалось купить улучшение управляемости.");
            RefreshCareerHub();
            return;
        }

        RefreshCareerHub();

        if (!CareerManager.SaveCareer(out string error))
        {
            Debug.LogError($"Улучшение куплено, но сохранение не удалось: {error}");
        }
    }

    private void BuyDriverSkillUpgrade()
    {
        var team = CareerManager.PlayerTeam;

        if (!team.Driver.CanUpgradeSkill)
        {
            Debug.Log("Не удалось купить улучшение навыка.");
            RefreshCareerHub();
            return;
        }
        team.Driver.UpgradeSkill();

        RefreshCareerHub();

        if (!CareerManager.SaveCareer(out string error))
        {
            Debug.LogError($"Улучшение куплено, но сохранение не удалось: {error}");
        }
    }

    private void BuyDriverAggressivenessUpgrade()
    {
        var team = CareerManager.PlayerTeam;

        if (!team.Driver.CanUpgradeAggressiveness)
        {
            Debug.Log("Не удалось купить улучшение навыка.");
            RefreshCareerHub();
            return;
        }
        team.Driver.UpgradeAggressiveness();

        RefreshCareerHub();

        if (!CareerManager.SaveCareer(out string error))
        {
            Debug.LogError($"Улучшение куплено, но сохранение не удалось: {error}");
        }
    }
}
