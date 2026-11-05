using RacingProject.Career;
using RacingProject.Domain;
using RacingProject.Simulation;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RaceUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject RacePanel;
    [SerializeField] private GameObject RaceResultsPanel;
    [SerializeField] private CareerUIController careerUIController;

    [Header("Race information")]
    [SerializeField] private TMP_Text TrackNameText;
    [SerializeField] private TMP_Text LapText;
    [SerializeField] private TMP_Text WeatherText;
    [SerializeField] private TMP_Text RaceEventText;

    [Header("Driver information")]
    [SerializeField] private TMP_Text DriverText;
    [SerializeField] private TMP_Text AggressivenessText;
    [SerializeField] private TMP_Text SkillText;
    [SerializeField] private TMP_Text StrategyText;

    [Header("Car condition")]
    [SerializeField] private TMP_Text CarCondition;
    [SerializeField] private TMP_Text TireType;
    [SerializeField] private TMP_Text TireCondition;
    [SerializeField] private TMP_Text Speed;
    [SerializeField] private TMP_Text Handling;

    [Header("Standings")]
    [SerializeField] private TMP_Text PlayerPositionText;
    [SerializeField] private TMP_Text Opponent1PositionText;
    [SerializeField] private TMP_Text Opponent2PositionText;
    [SerializeField] private TMP_Text Opponent3PositionText;

    [Header("Controls")]
    [SerializeField] private Button NextLapButton;
    [SerializeField] private Button FinishRaceButton;

    [Header("Results")]
    [SerializeField] private TMP_Text ResultsTitleText;
    [SerializeField] private TMP_Text ResultsTableText;
    [SerializeField] private TMP_Text FinalPlayerPositionText;
    [SerializeField] private TMP_Text PrizeReceivedText;
    [SerializeField] private TMP_Text ExpReceivedText;
    [SerializeField] private Button BackToCareerButton;


    private CareerManager careerManager;

    private void Awake()
    {
        NextLapButton.onClick.AddListener(RunNextLap);
        FinishRaceButton.onClick.AddListener(FinishRace);

        FinishRaceButton.interactable = false;

        BackToCareerButton.onClick.AddListener(BackToCareer);
        RaceResultsPanel.SetActive(false);
    }

    public void Initialize(CareerManager manager)
    {
        careerManager = manager;
    }

    public void ShowRace(string trackName)
    {
        if (careerManager == null || !careerManager.HasActiveRace)
        {
            Debug.LogError("No active race found");
            return;
        }

        RacePanel.SetActive(true);

        TrackNameText.text = trackName;

        NextLapButton.interactable = true;
        FinishRaceButton.interactable = false;

        UpdateRaceUI();
    }

    private void RunNextLap()
    {
        try
        {
            careerManager.RunNextLap();

            UpdateRaceUI();

            if (careerManager.CurrentRace.CurrentLap >= careerManager.CurrentRace.TotalLaps)
            {
                NextLapButton.interactable = false;
                FinishRaceButton.interactable = true;

                Debug.Log("All laps completed");
            }
        }
        catch (System.Exception exception)
        {
            Debug.LogError($"Could not run next lap: {exception.Message}");
        }
    }

    private void UpdateRaceUI()
    {
        Race race = careerManager.CurrentRace;

        LapText.text = $"Круг: {race.CurrentLap}/{race.TotalLaps}";
        DriverText.text = $"Пилот: {careerManager.PlayerTeam.Driver.Name}";
        AggressivenessText.text = $"Агрессивность: {careerManager.PlayerTeam.Driver.Aggressiveness.ToString()}";
        SkillText.text = $"Навык вождения: {careerManager.PlayerTeam.Driver.Skill.ToString()}";
        StrategyText.text = $"Стратегия: {careerManager.PlayerTeam.Strategy.Name}";

        CarCondition.text = $"Состояние болида: {careerManager.PlayerTeam.Car.Condition.ToString()}";
        TireType.text = $"Тип шин: {GetTiresName(careerManager.PlayerTeam.Car.Tire)}";
        TireCondition.text = $"Состояние шин: {careerManager.PlayerTeam.Car.Tire.Condition.ToString()}";
        Speed.text = $"Скорость болида: {careerManager.PlayerTeam.Car.Speed.ToString()}";
        Handling.text = $"Управляемость болида: {careerManager.PlayerTeam.Car.Handling.ToString()}";

        if(careerManager.CurrentRace.PendingEvent != null)
        {
            RaceEventText.text = careerManager.CurrentRace.PendingEvent.Description;
        }
        else
        {
            RaceEventText.text = "Гонка в процессе... Событий пока не происходило";
        }

        List<Team> positions = race.GetPositions();

        TMP_Text[] positionsText =
        {
            PlayerPositionText,
            Opponent1PositionText,
            Opponent2PositionText,
            Opponent3PositionText
        };

        for (int i = 0; i < positionsText.Length; i++)
        {
            if (i >= positions.Count)
            {
                positionsText[i].text = "";
                continue;
            }

            Team team = positions[i];
            TimeSpan elapsedTime = TimeSpan.FromSeconds(race.GetResult(team).Time);

            positionsText[i].text = $"{i + 1}. {team.Name} - {(int)elapsedTime.TotalMinutes:D2}:{elapsedTime.Seconds:D2}";
        }
    }

    private void FinishRace()
    {
        try
        {
            List<RaceResultEntry> results = careerManager.FinishCurrentRace();

            ShowResults(results);

            Debug.Log("Race finished successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Could not finish race: {e.Message}");
        }
    }

    private void ShowResults(List<RaceResultEntry> results)
    {
        RacePanel.SetActive(false);
        RaceResultsPanel.SetActive(true);

        ResultsTitleText.text = "РЕЗУЛЬТАТЫ ГОНКИ";

        System.Text.StringBuilder table = new();

        table.AppendLine("ПОЗИЦИИ | КОМАНДА | ВРЕМЯ | ПРИЗОВЫЕ");
        table.AppendLine("----------------------------------------------");

        for (int i = 0; i < results.Count; i++)
        {
            RaceResultEntry entry = results[i];
            table.AppendLine($"{i + 1}. {entry.Team.Name} | {FormatTime(entry.Result.Time)} | {entry.Result.Prize:N0}");

            if (entry.Team.IsPlayerControlled)
            {
                FinalPlayerPositionText.text = $"ТВОЕ МЕСТО: {i + 1}";
                PrizeReceivedText.text = $"ПОЛУЧЕНО: {entry.Result.Prize}";
                ExpReceivedText.text = $"+{entry.Result.Experience}";
            }
        }

        ResultsTableText.text = table.ToString();
    }

    private string FormatTime(double seconds)
    {
        TimeSpan span = TimeSpan.FromSeconds(seconds);

        return $"{(int)span.TotalMinutes:D2}:{span.Seconds:D2}.{span.Milliseconds / 10:D2}";
    }

    private void BackToCareer()
    {
        if (careerManager.SaveCareer(out string error))
        {
            Debug.Log("Career saved successfully");
        }
        else
        {
            Debug.LogError($"Couldn't save career: {error}");
        }

        RaceResultsPanel.SetActive(false);

        careerUIController.ShowCareerHub();
    }

    private string GetTiresName(Tire tire)
    {
        if (tire.Name == "Soft")
        {
            return "Мягкие";
        }
        else if (tire.Name == "Medium")
        {
            return "Средние";
        }
        else if (tire.Name == "Hard")
        {
            return "Жесткие";
        }

        return "Неизвестно";
    }
}
