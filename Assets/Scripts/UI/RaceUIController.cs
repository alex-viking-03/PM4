using RacingProject.Career;
using RacingProject.Domain;
using RacingProject.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RaceUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject RacePanel;
    [SerializeField] private GameObject RaceResultsPanel;
    [SerializeField] private GameObject StrategyButtonsPanel;
    [SerializeField] private GameObject PitStopButtonsPanel;
    [SerializeField] private CareerUIController careerUIController;

    [Header("Race information")]
    [SerializeField] private TMP_Text TrackNameText;
    [SerializeField] private TMP_Text LapText;
    [SerializeField] private TMP_Text WeatherText;
    [SerializeField] private TMP_Text RaceEventText;
    [SerializeField] private Button OKButton;

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
    [SerializeField] private TMP_Text TeamNamesText;
    [SerializeField] private TMP_Text FinalTimeText;
    [SerializeField] private TMP_Text PrizeText;
    [SerializeField] private TMP_Text FinalPlayerPositionText;
    [SerializeField] private TMP_Text PrizeReceivedText;
    [SerializeField] private TMP_Text ExpReceivedText;
    [SerializeField] private Button BackToCareerButton;

    [Header("Strategy")]
    [SerializeField] private Button AttackButton;
    [SerializeField] private Button HoldPositionButton;
    [SerializeField] private Button ConserveButton;

    [Header("Pit stop")]
    [SerializeField] private Button SoftTiresButton;
    [SerializeField] private Button MediumTiresButton;
    [SerializeField] private Button HardTiresButton;

    private CareerManager careerManager;

    private void Awake()
    {
        NextLapButton.onClick.AddListener(RunNextLap);
        FinishRaceButton.onClick.AddListener(FinishRace);

        AttackButton.onClick.AddListener(AttackDecision);
        HoldPositionButton.onClick.AddListener(HoldPositionDecision);
        ConserveButton.onClick.AddListener(ConserveDecision);

        SoftTiresButton.onClick.AddListener(PitStopSoft);
        MediumTiresButton.onClick.AddListener(PitStopMedium);
        HardTiresButton.onClick.AddListener(PitStopHard);

        FinishRaceButton.interactable = false;

        BackToCareerButton.onClick.AddListener(BackToCareer);
        RaceResultsPanel.SetActive(false);

        OKButton.onClick.AddListener(AcknowledgeRaceEvent);
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

        RaceEventText.text = "";

        UpdateRaceUI();
    }

    private void RunNextLap()
    {
        RaceEventText.text = "";
        try
        {
            careerManager.RunNextLap();

            UpdateRaceUI();

            if (careerManager.CurrentRace.CurrentLap >= careerManager.CurrentRace.TotalLaps)
            {
                NextLapButton.interactable = false;
                FinishRaceButton.interactable = true;
            }
        }
        catch (Exception exception)
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

        RaceEvent pendingEvent = careerManager.CurrentRace.PendingEvent;
        bool hasPendingEvent = pendingEvent != null;

        if(hasPendingEvent)
        {
            RaceEventText.text = pendingEvent.Description;
        }

        NextLapButton.interactable = !hasPendingEvent;

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
            double time = race.GetResult(team).Time;

            positionsText[i].text =
                $"{i + 1}. {team.Name} | {FormatTime(time)}";
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
        catch (Exception e)
        {
            Debug.LogError($"Could not finish race: {e.Message}");
        }
    }

    private void AttackDecision()
    {
        ResolveOvertaking(OvertakingDecision.ATTACK);
    }

    private void HoldPositionDecision()
    {
        ResolveOvertaking(OvertakingDecision.HOLD_POSITION);
    }

    private void ConserveDecision()
    {
        ResolveOvertaking(OvertakingDecision.CONSERVES);
    }

    private void ResolveOvertaking(OvertakingDecision decision)
    {
        try
        {
            Team playerTeam = careerManager.CurrentRace.GetPositions().FirstOrDefault(team => team.IsPlayerControlled);

            switch (decision)
            {
                case OvertakingDecision.ATTACK:
                    playerTeam.SetStrategy(Strategy.ATTACK);
                    RaceEventText.text = "Стратегия изменена на Атаку. Теперь ваш гонщик ведет машину агрессивнее, а шины изнашиваются быстрее";
                    break;

                case OvertakingDecision.HOLD_POSITION:
                    playerTeam.SetStrategy(Strategy.NORMAL);
                    RaceEventText.text = "Стратегия изменена на Баланс. Теперь ваш гонщик ведет машину сбалансированно, а шины изнашиваются как при обычной езде";
                    break;

                case OvertakingDecision.CONSERVES:
                    playerTeam.SetStrategy(Strategy.CONSERVE);
                    RaceEventText.text = "Стратегия изменена на Экономию. Теперь ваш гонщик ведет машину спокойнее, а шины изнашиваются медленее";
                    break;
            }

            UpdateRaceUI();
        }
        catch(Exception exxception)
        {
            Debug.LogError("Could not resovle player decision: " +  exxception.Message);
        }
    }

    private void PitStopSoft()
    {
        PitStop(new Soft());
    }

    private void PitStopMedium()
    {
        PitStop(new Medium());
    }

    private void PitStopHard()
    {
        PitStop(new Hard());
    }

    private void PitStop(Tire tire)
    {
        try
        {
            careerManager.CurrentRace.PitStop(careerManager.PlayerTeam, tire);
            RaceEventText.text = "Пит стоп удался! Вам прибавлено 20 сек";

            UpdateRaceUI();
        }
        catch (Exception exception)
        {
            Debug.LogError($"Cannot use defence decision: {exception.Message}");
        }
    }

    private void ShowResults(List<RaceResultEntry> results)
    {
        RacePanel.SetActive(false);
        RaceResultsPanel.SetActive(true);

        ResultsTitleText.text = "РЕЗУЛЬТАТЫ ГОНКИ";

        for (int i = 0; i < results.Count; i++)
        {
            RaceResultEntry entry = results[i];

            TeamNamesText.text += $"{entry.Team.Name}\n";
            FinalTimeText.text += $"{FormatTime(entry.Result.Time)}\n";
            PrizeText.text += $"{entry.Result.Prize}\n";

            if (entry.Team.IsPlayerControlled)
            {
                FinalPlayerPositionText.text = $"ТВОЕ МЕСТО: {i + 1}";
                PrizeReceivedText.text = $"ПОЛУЧЕНО: {entry.Result.Prize}";
                ExpReceivedText.text = $"Опыта получено: +{entry.Result.Experience}";
            }
        }
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

    private void AcknowledgeRaceEvent()
    {
        careerManager.CurrentRace.AcknowledgeEvent();
        NextLapButton.interactable = true;
    }
}
