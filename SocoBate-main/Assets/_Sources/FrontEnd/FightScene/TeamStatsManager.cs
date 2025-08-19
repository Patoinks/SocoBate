// TeamStatsManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Context; // Assuming you still use this for nicknames
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class TeamStatsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private ScrollRect playerScrollView;
    [SerializeField] private ScrollRect enemyScrollView;
    [SerializeField] private GameObject unitRowPrefab;
    [SerializeField] private TMP_Text nickname1;
    [SerializeField] private TMP_Text nickname2;
    [SerializeField] private TMP_Text winner1;
    [SerializeField] private TMP_Text winner2;

    // --- Private State ---
    private string _currentStatType = "DamageDealt"; // Default stat to display
    private readonly List<UnitStatsRow> _playerRows = new List<UnitStatsRow>();
    private readonly List<UnitStatsRow> _enemyRows = new List<UnitStatsRow>();
    private List<BaseUnit> _allUnitsInDataFormat = new List<BaseUnit>();

    // This method is called when the EndBattleMenu is set active.
    void OnEnable()
    {
        InitializeManager();
    }

    private void InitializeManager()
    {
        Debug.Log("Initializing TeamStatsManager...");

        // Safety check for the singleton
        if (BattleManager.Instance == null)
        {
            Debug.LogError("BattleManager.Instance is not available! Cannot populate stats screen.");
            return;
        }

        SetupHeaderUI(BattleManager.Instance);
        PopulateStatRows(BattleManager.Instance);
    }

    private void SetupHeaderUI(BattleManager manager)
    {
        // Set nicknames from your context scripts
        if (nickname1 != null) nickname1.text = UserContext.account.Nickname;
        if (nickname2 != null) nickname2.text = FightContext.OpponentNickName;

        // Set the winner text based on the result from the BattleManager
        if (manager.WinnerTeam == "Players")
        {
            winner1.text = "Winner";
            winner2.text = "Loser";
        }
        else
        {
            winner1.text = "Loser";
            winner2.text = "Winner";
        }
    }

    private void PopulateStatRows(BattleManager manager)
    {
        if (unitRowPrefab == null)
        {
            Debug.LogError("unitRowPrefab is NOT assigned in TeamStatsManager!");
            return;
        }

        ClearExistingRows();

        // Get all units that participated in the battle from the BattleManager
        var allPlayerUnits = manager.PlayerUnitsAtEnd;
        var allEnemyUnits = manager.EnemyUnitsAtEnd;

        // Convert facades to BaseUnit data for easier processing
        var allPlayerUnitData = allPlayerUnits.Select(facade => facade.UnitData).ToList();
        var allEnemyUnitData = allEnemyUnits.Select(facade => facade.UnitData).ToList();
        _allUnitsInDataFormat = allPlayerUnitData.Concat(allEnemyUnitData).ToList();

        // Calculate sliders based on the default stat type
        UpdateAndDisplayStats();
    }
    
    private void UpdateAndDisplayStats()
    {
        float maxValue = GetMaxStatValueForCurrentType();
        float minValue = 0; // Stats like damage can't be negative, so min is 0

        // If this is the first time populating, instantiate the rows
        if (_playerRows.Count == 0 && _enemyRows.Count == 0)
        {
            // Get the unit data from our stored list
            var allPlayerUnitData = _allUnitsInDataFormat.Where(u => BattleManager.Instance.PlayerUnitsAtEnd.Any(f => f.UnitData == u)).ToList();
            var allEnemyUnitData = _allUnitsInDataFormat.Where(u => BattleManager.Instance.EnemyUnitsAtEnd.Any(f => f.UnitData == u)).ToList();

            foreach (var unitData in allPlayerUnitData)
            {
                InstantiateUnitRow(unitData, playerScrollView.content, maxValue, minValue, _playerRows);
            }
            foreach (var unitData in allEnemyUnitData)
            {
                InstantiateUnitRow(unitData, enemyScrollView.content, maxValue, minValue, _enemyRows);
            }
        }
        else // Otherwise, just update the existing rows
        {
            foreach (var row in _playerRows.Concat(_enemyRows))
            {
                row.Initialize(row.unit, maxValue, minValue, _currentStatType);
            }
        }
    }


    private void InstantiateUnitRow(BaseUnit unit, Transform parent, float maxValue, float minValue, List<UnitStatsRow> rowList)
    {
        GameObject rowGO = Instantiate(unitRowPrefab, parent);
        UnitStatsRow rowComponent = rowGO.GetComponent<UnitStatsRow>();
        if (rowComponent != null)
        {
            rowComponent.Initialize(unit, maxValue, minValue, _currentStatType);
            rowList.Add(rowComponent);
        }
    }

    private void ClearExistingRows()
    {
        foreach (var row in _playerRows.Concat(_enemyRows))
        {
            Destroy(row.gameObject);
        }
        _playerRows.Clear();
        _enemyRows.Clear();
    }

    #region Stat Calculation Helpers

    private float GetMaxStatValueForCurrentType()
    {
        if (_allUnitsInDataFormat.Count == 0) return 0;
        return _allUnitsInDataFormat.Max(GetStatValueFromUnit);
    }

    private float GetStatValueFromUnit(BaseUnit unit)
    {
        switch (_currentStatType)
        {
            case "DamageDealt": return unit.damageDealt;
            case "DamageTaken": return unit.damageTaken;
            case "Healing": return unit.healingDone;
            default: return 0;
        }
    }

    #endregion

    #region Public UI Button Handlers

    public void OnClickChangeScene()
    {
        // It's good practice to reset Time.timeScale if you ever change it
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void SetStatType(string statType)
    {
        if (_currentStatType == statType) return; // No change needed

        _currentStatType = statType;
        Debug.Log($"Changing displayed stat to: {_currentStatType}");
        UpdateAndDisplayStats();
    }

    // You can link these to your UI Buttons' OnClick() events in the Inspector
    public void ChangeStatToDamageDealt() => SetStatType("DamageDealt");
    public void ChangeStatToDamageTaken() => SetStatType("DamageTaken");
    public void ChangeStatToHealing() => SetStatType("Healing");
    
    #endregion
}