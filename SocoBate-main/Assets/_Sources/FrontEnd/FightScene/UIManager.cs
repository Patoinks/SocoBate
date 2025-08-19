// UIManager.cs (Definitive, with Stats Panel Logic)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Context;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Main UI")]
    [SerializeField] private Slider playerTotalHealthSlider;
    [SerializeField] private Slider enemyTotalHealthSlider;
    [SerializeField] private Button speedButton;
    [SerializeField] private TMP_Text speedButtonText;
    [SerializeField] private Button skipButton;
    [SerializeField] private TMP_Text player1NameText;
    [SerializeField] private TMP_Text player2NameText;

    [Header("Stats Panel")]
    [Tooltip("Assign your 'InGameAttrb' prefab here.")]
    [SerializeField] private GameObject statsPanelPrefab; 
    [SerializeField] private Canvas mainCanvas; // The canvas where the panel will be created

    public static bool IsFightSkipped { get; private set; }
    
    // Private variables to manage the stats panel
    private GameObject _statsPanelInstance;
    private UnitFacade _currentlyDisplayedUnit;
    
    private int _currentSpeedMultiplier = 1;
    private float _playerTotalMaxHp;
    private float _enemyTotalMaxHp;
    private bool _initialHpCalculated = false;

    void Awake() 
    { 
        if (Instance != null && Instance != this) { Destroy(gameObject); return; } 
        Instance = this; 
        IsFightSkipped = false; 
    }

    void Start()
    {
        if (player1NameText != null) player1NameText.text = UserContext.account.Nickname;
        if (player2NameText != null) player2NameText.text = FightContext.OpponentNickName;
        if (speedButton != null) { speedButton.onClick.AddListener(OnSpeedButtonClicked); UpdateSpeedButtonText(); }
        if (skipButton != null) skipButton.onClick.AddListener(OnSkipButtonClicked);
        Time.timeScale = 1f;
    }

    void Update()
    {
        UpdateTotalHealthBars();
        
        // This ensures the stats panel shows real-time data if a unit's stats change.
        if (_statsPanelInstance != null && _statsPanelInstance.activeInHierarchy && _currentlyDisplayedUnit != null)
        {
            UpdateStatsPanelContents(_currentlyDisplayedUnit);
        }
    }

    #region Stats Panel Logic (New and Corrected)
    /// <summary>
    /// This is the main public method, called by UnitFacade when a unit is clicked.
    /// </summary>
    public void ShowStatsFor(UnitFacade unit)
    {
        _currentlyDisplayedUnit = unit;

        // If the panel has never been created, instantiate it now.
        if (_statsPanelInstance == null)
        {
            if (statsPanelPrefab == null || mainCanvas == null) { Debug.LogError("Stats Panel Prefab or Main Canvas is not assigned in UIManager!"); return; }
            
            _statsPanelInstance = Instantiate(statsPanelPrefab, mainCanvas.transform);
            
            // This assumes your panel has a button to close it. If not, clicking another unit will just switch the stats.
            // You can add a "CloseButton" to your prefab for this to work.
            Button closeButton = _statsPanelInstance.transform.Find("CloseButton")?.GetComponent<Button>();
            if (closeButton != null) 
            {
                closeButton.onClick.AddListener(HideStatsPanel);
            }
        }
        
        _statsPanelInstance.SetActive(true);
        UpdateStatsPanelContents(unit);
    }

    private void HideStatsPanel()
    {
        if (_statsPanelInstance != null)
        {
            _statsPanelInstance.SetActive(false);
            _currentlyDisplayedUnit = null;
        }
    }

    /// <summary>
    /// Populates the text fields on the panel using the data from the selected unit.
    /// The names ("HP", "STR", etc.) must match the names of the child GameObjects in your prefab.
    /// </summary>
    private void UpdateStatsPanelContents(UnitFacade unit)
    {
        BaseUnit data = unit.UnitData;
        SetText(_statsPanelInstance.transform, "HP", $"HP: {data.baseHp}/{data.maxHp}");
        SetText(_statsPanelInstance.transform, "STR", $"STR: {data.baseStr}");
        SetText(_statsPanelInstance.transform, "MDEF", $"MDEF: {data.mDef}");
        SetText(_statsPanelInstance.transform, "NAME", $"NAME: {data.unitName}");
        SetText(_statsPanelInstance.transform, "SPEED", $"SPEED: {data.baseSpeed}");
        SetText(_statsPanelInstance.transform, "PDEF", $"PDEF: {data.pDef}");
        SetText(_statsPanelInstance.transform, "INT", $"INT: {data.baseInt}");
        SetText(_statsPanelInstance.transform, "AURA", $"AURA: {data.aura}");
    }

    // A helper function to safely find and set text on a child object.
    private void SetText(Transform parent, string childName, string value)
    {
        TMP_Text textComponent = parent.Find(childName)?.GetComponent<TMP_Text>();
        if (textComponent != null) 
        {
            textComponent.text = value;
        }
    }
    #endregion

    #region Main UI Logic (Unchanged)
    private void UpdateTotalHealthBars()
    {
        if (UnitRegistry.Instance == null) return;
        var allPlayerUnits = UnitRegistry.Instance.GetPlayerUnits(true);
        var allEnemyUnits = UnitRegistry.Instance.GetEnemyUnits(true);
        if (!_initialHpCalculated && allPlayerUnits.Any() && allEnemyUnits.Any())
        {
            _playerTotalMaxHp = allPlayerUnits.Sum(u => u.UnitData.maxHp);
            _enemyTotalMaxHp = allEnemyUnits.Sum(u => u.UnitData.maxHp);
            if (playerTotalHealthSlider != null) playerTotalHealthSlider.maxValue = _playerTotalMaxHp;
            if (enemyTotalHealthSlider != null) enemyTotalHealthSlider.maxValue = _enemyTotalMaxHp;
            _initialHpCalculated = true;
        }
        if (_initialHpCalculated)
        {
            float playerCurrentHp = allPlayerUnits.Sum(u => u.UnitData.baseHp);
            float enemyCurrentHp = allEnemyUnits.Sum(u => u.UnitData.baseHp);
            if (playerTotalHealthSlider != null) playerTotalHealthSlider.value = playerCurrentHp;
            if (enemyTotalHealthSlider != null) enemyTotalHealthSlider.value = enemyCurrentHp;
        }
    }
    private void OnSpeedButtonClicked()
    {
        switch (_currentSpeedMultiplier) { case 1: _currentSpeedMultiplier = 2; break; case 2: _currentSpeedMultiplier = 4; break; case 4: _currentSpeedMultiplier = 1; break; default: _currentSpeedMultiplier = 1; break; }
        Time.timeScale = _currentSpeedMultiplier;
        UpdateSpeedButtonText();
    }
    private void UpdateSpeedButtonText() { if (speedButtonText != null) speedButtonText.text = $"{_currentSpeedMultiplier}x"; }
    private void OnSkipButtonClicked()
    {
        if (IsFightSkipped) return;
        IsFightSkipped = true;
        if (skipButton != null) skipButton.interactable = false;
        if (speedButton != null) speedButton.interactable = false;
        BattleManager.Instance.SimulateToEndOfBattle();
    }
    #endregion
}