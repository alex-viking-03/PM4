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
    [SerializeField] private Image TrackImage;
    [SerializeField] private Sprite[] TrackSprites;

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

    [Header("Track selection")]
    [SerializeField] private Button PreviousTrackButton;
    [SerializeField] private Button NextTrackButton;

    [Header("Controllers")]
    [SerializeField] private RaceUIController raceController;

    [Header("Parametres preview")]
    [SerializeField] private Image TireImage;
    
    [SerializeField] private Sprite SoftTireSprite;
    [SerializeField] private Sprite MediumTireSprite;
    [SerializeField] private Sprite HardTireSprite;

    [SerializeField] private TMP_Text TiresDescriptionText;

    [SerializeField] private Image StrategyImage;

    [SerializeField] private Sprite NormalStrategySprite;
    [SerializeField] private Sprite AttackStrategySprite;
    [SerializeField] private Sprite ConserveStrategySprite;

    [SerializeField] private TMP_Text StrategyDescriptionText;

    private CareerManager careerManager;

    private Strategy selectedStrategy = Strategy.NORMAL;
    private Tire selectedTires = new Medium();

    private List<Track> tracks;
    private Track selectedTrack;
    private int selectedTrackIndex;
    private List<double> prizes;

    private readonly Color selectedColor = new Color(0.35f, 0.75f, 0.45f);

    private readonly Color normalColor = Color.white;

    private void Awake()
    {

        tracks = new List<Track>
        {
            new Track("PM-4 GT Circles", 5.2, 10, 5),
            new Track("Desert Circuit", 4.6, 12, 6),
            new Track("Mountain Ring", 6.1, 9, 8),
            new Track("Grand Prix Circuit", 7.0, 10, 10)
        };

        selectedTrackIndex = 0;
        selectedTrack = tracks[selectedTrackIndex];

        prizes = new List<double>{
            10000,
            6000,
            3000
        };

        PreviousTrackButton.onClick.AddListener(PreviousTrack);
        NextTrackButton.onClick.AddListener(NextTrack);

        StrategyAttackButton.onClick.AddListener(
            () => SelectStrategy(Strategy.ATTACK)
        );

        StrategyNormalButton.onClick.AddListener(
            () => SelectStrategy(Strategy.NORMAL)
        );

        StrategyConserveButton.onClick.AddListener(
            () => SelectStrategy(Strategy.CONSERVE)
        );

        SoftTiresButton.onClick.AddListener(
            () => SelectTires(new Soft())
        );

        MediumTiresButton.onClick.AddListener(
            () => SelectTires(new Medium())
        );

        HardTiresButton.onClick.AddListener(
            () => SelectTires(new Hard())
        );

        StartRaceButton.onClick.AddListener(StartRace);
        BackToHubButton.onClick.AddListener(BackToCareerHub);

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

    private void PreviousTrack()
    {
        selectedTrackIndex--;

        if (selectedTrackIndex < 0)
            selectedTrackIndex = tracks.Count - 1;

        SelectTrack();
    }

    private void NextTrack()
    {
        selectedTrackIndex++;

        if (selectedTrackIndex >= tracks.Count)
            selectedTrackIndex = 0;

        SelectTrack();
    }

    private void SelectTrack()
    {
        selectedTrack = tracks[selectedTrackIndex];
        UpdateTrackInfo();
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
        TrackInfoText.text = $"Длина круга: {selectedTrack.Length} км\n" +
                             $"Кругов: {selectedTrack.Laps}\n" +
                             $"Сложность: {selectedTrack.Difficulty}";

        TrackImage.sprite = TrackSprites[selectedTrackIndex];
    }

    private void UpdateSelectionUI()
    {
        StrategyText.text = $"Стратегия: {selectedStrategy.Name}";
        TiresText.text = $"Шины: {GetTiresName(selectedTires)}";

        SetButtonColor(StrategyAttackButton, selectedStrategy == Strategy.ATTACK);
        SetButtonColor(StrategyNormalButton, selectedStrategy == Strategy.NORMAL);
        SetButtonColor(StrategyConserveButton, selectedStrategy == Strategy.CONSERVE);
        SetButtonColor(SoftTiresButton, selectedTires is Soft);
        SetButtonColor(MediumTiresButton, selectedTires is Medium);
        SetButtonColor(HardTiresButton, selectedTires is Hard);

        if (selectedTires is Soft)
        {
            TireImage.sprite = SoftTireSprite;
            TiresDescriptionText.text = "Самые быстрые\n\nБыстро изнашиваются";
        }
        else if (selectedTires is Medium)
        {
            TireImage.sprite = MediumTireSprite;
            TiresDescriptionText.text = "Сбалансированные";
        }
        else if (selectedTires is Hard)
        {
            TireImage.sprite = HardTireSprite;
            TiresDescriptionText.text = "Самые медленные\n\nНаиболее долговечные";
        }

        if(selectedStrategy == Strategy.ATTACK)
        {
            StrategyImage.sprite = AttackStrategySprite;
            StrategyDescriptionText.text = "Максимальная скорость\n\nВысокий износ шин\n\nНаибольший риск";
        }
        else if(selectedStrategy == Strategy.NORMAL)
        {
            StrategyImage.sprite = NormalStrategySprite;
            StrategyDescriptionText.text = "Сбалансированный темп\n\nСредний износ шин\n\nСредний риск";
        }
        else if(selectedStrategy == Strategy.CONSERVE)
        {
            StrategyImage.sprite = ConserveStrategySprite;
            StrategyDescriptionText.text = "Меньшая скорость\n\n\nМеньший износ\n\nНаименьший риск";
        }
    }

    private string GetTiresName(Tire tire)
    {
        if(tire.Name == "Soft")
        {
            return "Мягкие";
        }
        else if(tire.Name == "Medium")
        {
            return "Средние";
        }
        else if(tire.Name == "Hard")
        {
            return "Жесткие";
        }

        return "Неизвестно";
    }

    private void SetButtonColor(Button button, bool selected)
    {
        if (button != null && button.image != null)
        {
            button.image.color = selected ? selectedColor : normalColor;
        }
    }

    private void StartRace()
    {
        if (careerManager == null ||
           careerManager.PlayerTeam == null)
        {
            Debug.LogError("Career has not been initialized");
            return;
        }
        try
        {
            Team playerTeam = careerManager.PlayerTeam;

            playerTeam.SetStrategy(selectedStrategy);
            playerTeam.Car.ChangeTire(selectedTires);

            careerManager.StartNextRace(selectedTrack, prizes);

            raceController.Initialize(careerManager);
            raceController.ShowRace(selectedTrack.Name);

            RacePreparationPanel.SetActive(false);

            Debug.Log(
                $"Race started: {selectedTrack.Name};\n" +
                $"Strategy: {selectedStrategy.Name};\n" +
                $"Tires: {selectedTires.Name}"
            );
        }
        catch(Exception exception)
        {
            Debug.LogError($"Could not start race: {exception.Message}");
        }
    }

    public void BackToCareerHub()
    {
        if (careerManager != null &&
           careerManager.HasActiveRace)
        {
            Debug.LogWarning("A race has started. You can't leave active race");
            return;
        }

        RacePreparationPanel.SetActive(false);
        CareerHubPanel.SetActive(true);
    }
}
