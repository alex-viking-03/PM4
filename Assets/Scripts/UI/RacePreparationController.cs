using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using RacingProject.Domain;
using RacingProject.Career;
using UnityEngine;

public class RacePreparationController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject CareerHubPanel;
    [SerializeField] private GameObject RacePreparationPanel;

    [Header("Track")]
    [SerializeField] private TMP_Text TrackNameText;
    [SerializeField] private TMP_Text TrackInfoText;

    [Header("Options")]
    [SerializeField] private TMP_Text StrategyText;
    [SerializeField] private TMP_Text TiresText;

    [Header("Strategy buttons")]
    [SerializeField] private Button StrategyAttackButton;
    [SerializeField] private Button StrategyNormalButton;
    [SerializeField] private Button StrategyConserveButton;

    [Header("Tire buttons")]
    [SerializeField] private Button SoftTiresButton;
    [SerializeField] private Button MediumTiresButton;
    [SerializeField] private Button HardTiresButton;

    [Header("Action buttons")]
    [SerializeField] private Button StartRaceButton;
    [SerializeField] private Button BackToHubButton;

    private CareerManager careerManager;

    private Strategy selectedStrategy = Strategy.NORMAL;
    private Tire selectedTires = new Medium();

    private Track selectedTrack;
    private List<double> prizes;

    private readonly Color selectedColor = new Color(0.35f, 0.75f, 0.45f);

    private readonly Color normalColor = Color.white;

    private void Awake()
    {
        selectedTrack = new Track(
            "PM-4 GT Circles",
            5.2,
            10,
            5
        );

        prizes = new List<double>{
            10000,
            6000,
            3000
        };

        StrategyAttackButton.OnClick.AddListener(
            () => SelectStrategy(Strategy.ATTACK)
        );

        StrategyNormalButton.OnClick.AddListener(
            () => SelectStrategy(Strategy.NORMAL)
        );

        StrategyConserveButton.OnClick.AddListener(
            () => SelectStrategy(Strategy.CONSERVE)
        );

        SoftTiresButton.OnClick.AddListener(
            () => SelectTires(new Soft())
        );

        MediumTiresButton.OnClick.AddListener(
            () => SelectTires(new Medium())
        );

        HardTiresButton.OnClick.AddListener(
            () => SelectTires(new Hard())
        );

        StartRaceButton.OnClick.AddListener(StartRace);
        BackToHubButton.OnClick.AddListener(BackToCareerHub);

        UpdateTrackInfo();
        UpdateSelectionUI();
    }

    public void Initialize(CareerManager manager)
    {
        careerManager = manager;
    }

    public void ShowRacePreparation()
    {
        if(careerManager == null ||
           careerManager.PlayerTeam == null)
        {
            Debug.LogError("Career has not been initialized");
            return;
        }

        RacePreparationPanel.SetActive(true);
        CareerHubPanel.SetActive(false);

        selectedStrategy = careerManager.PlayerTeam.Strategy;
        selectedTires = careerManager.PlayerTeam.Car.Tire;

        UpdateSelectionUI();
    }

    private void SelectStrategy(Strategy strategy)
    {
        selectedStrategy = strategy;
        UpdateSelectionUI();
    }

    private void SelectTires(Tire tire)
    {
        selectedTires = tire;
        UpdateSelectionUI();
    }

    private void UpdateTrackInfo()
    {
        TrackNameText.text = selectedTrack.Name;
        TrackInfoText.text = $"Длина круга: {selectedTrack.Length}\n км" +
                             $"Кругов: {selectedTrack.Laps}\n" +
                             $"Сложность: {selectedTrack.Difficulty}";
    }
}
