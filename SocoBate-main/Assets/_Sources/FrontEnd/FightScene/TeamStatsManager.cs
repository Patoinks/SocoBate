using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamStatsManager : MonoBehaviour
{
    public ScrollRect playerScrollView;
    public ScrollRect enemyScrollView;
    public GameObject unitRowPrefab;
    public TMP_Text displayText;

    private List<BaseUnit> playerAliveUnits;
    private List<BaseUnit> playerDeadUnits;
    private List<BaseUnit> enemyAliveUnits;
    private List<BaseUnit> enemyDeadUnits;
    private string currentStat = "DamageDealt";  // Default stat
    private List<UnitStatsRow> playerRows = new List<UnitStatsRow>();
    private List<UnitStatsRow> enemyRows = new List<UnitStatsRow>();

    void Start()
    {
        Debug.Log("Started TeamStatsManager");
        FetchUnitsFromDuelScript();
    }

    public void FetchUnitsFromDuelScript()
    {
        DuelScript duelScript = FindObjectOfType<DuelScript>();
        if (duelScript != null)
        {
            // Fetch separate lists for alive and dead units
            playerAliveUnits = duelScript.playerUnits;
            playerDeadUnits = duelScript.deadPlayerUnits;
            enemyAliveUnits = duelScript.enemyUnits;
            enemyDeadUnits = duelScript.deadEnemyUnits;

            InitializeRows();
        }
        else
        {
            Debug.LogError("DuelScript not found!");
        }
    }

    void InitializeRows()
    {
        if (unitRowPrefab == null)
        {
            Debug.LogError("unitRowPrefab is NOT assigned in TeamStatsManager!");
            return;
        }

        // Clear any existing rows before initializing
        ClearExistingRows();

        // Calculate the max and min values for the current stat (DamageDealt, DamageTaken, Healing)
        float maxValue = GetMaxStatValue();
        float minValue = GetMinStatValue();

        // Instantiate rows for alive player units
        foreach (BaseUnit unit in playerAliveUnits)
        {
            InstantiateUnitRow(unit, playerScrollView.content, maxValue, minValue);
        }

        // Instantiate rows for dead player units
        foreach (BaseUnit unit in playerDeadUnits)
        {
            InstantiateUnitRow(unit, playerScrollView.content, maxValue, minValue);
        }

        // Instantiate rows for alive enemy units
        foreach (BaseUnit unit in enemyAliveUnits)
        {
            InstantiateUnitRow(unit, enemyScrollView.content, maxValue, minValue);
        }

        // Instantiate rows for dead enemy units
        foreach (BaseUnit unit in enemyDeadUnits)
        {
            InstantiateUnitRow(unit, enemyScrollView.content, maxValue, minValue);
        }
    }

    void ClearExistingRows()
    {
        // Clear player rows
        foreach (var row in playerRows)
        {
            Destroy(row.gameObject);
        }
        playerRows.Clear();

        // Clear enemy rows
        foreach (var row in enemyRows)
        {
            Destroy(row.gameObject);
        }
        enemyRows.Clear();
    }

    void InstantiateUnitRow(BaseUnit unit, Transform parent, float maxValue, float minValue)
    {
        GameObject row = Instantiate(unitRowPrefab, parent);
        UnitStatsRow rowComponent = row.GetComponent<UnitStatsRow>();
        if (rowComponent != null)
        {
            // Add row to the respective list
            if (parent == playerScrollView.content)
            {
                playerRows.Add(rowComponent);
            }
            else
            {
                enemyRows.Add(rowComponent);
            }

            // Initialize the row with the current stat
            rowComponent.Initialize(unit, maxValue, minValue, currentStat);
        }
        else
        {
            Debug.LogError("UnitStatsRow component not found in prefab!");
        }
    }

    float GetMaxStatValue()
    {
        float maxValue = float.MinValue;

        // Check max value for player units
        foreach (BaseUnit unit in playerAliveUnits)
        {
            maxValue = Mathf.Max(maxValue, GetStatValue(unit));
        }

        // Check max value for enemy units
        foreach (BaseUnit unit in enemyAliveUnits)
        {
            maxValue = Mathf.Max(maxValue, GetStatValue(unit));
        }

        // Check max value for dead player units
        foreach (BaseUnit unit in playerDeadUnits)
        {
            maxValue = Mathf.Max(maxValue, GetStatValue(unit));
        }

        // Check max value for dead enemy units
        foreach (BaseUnit unit in enemyDeadUnits)
        {
            maxValue = Mathf.Max(maxValue, GetStatValue(unit));
        }

        return maxValue;
    }

    float GetMinStatValue()
    {
        float minValue = float.MaxValue;

        // Check min value for player units
        foreach (BaseUnit unit in playerAliveUnits)
        {
            minValue = Mathf.Min(minValue, GetStatValue(unit));
        }

        // Check min value for enemy units
        foreach (BaseUnit unit in enemyAliveUnits)
        {
            minValue = Mathf.Min(minValue, GetStatValue(unit));
        }

        // Check min value for dead player units
        foreach (BaseUnit unit in playerDeadUnits)
        {
            minValue = Mathf.Min(minValue, GetStatValue(unit));
        }

        // Check min value for dead enemy units
        foreach (BaseUnit unit in enemyDeadUnits)
        {
            minValue = Mathf.Min(minValue, GetStatValue(unit));
        }

        return minValue;
    }

    float GetStatValue(BaseUnit unit)
    {
        switch (currentStat)
        {
            case "DamageDealt":
                return unit.damageDealt;
            case "DamageTaken":
                return unit.damageTaken;
            case "Healing":
                return unit.healingDone;
            default:
                return unit.damageDealt;  // Default to DamageDealt
        }
    }

    void UpdateUnitStats()
    {
        displayText.text = "Current stat: " + currentStat;

        // Calculate the max and min values for the current stat (DamageDealt, DamageTaken, Healing)
        float maxValue = GetMaxStatValue();
        float minValue = GetMinStatValue();

        // Update the stats in all rows based on the current stat
        foreach (UnitStatsRow row in playerRows)
        {
            row.UpdateStat(GetStatValue(row.unit)); // Update player rows
        }

        foreach (UnitStatsRow row in enemyRows)
        {
            row.UpdateStat(GetStatValue(row.unit)); // Update enemy rows
        }
    }

    public void ChangeStatToDamageDealt() { currentStat = "DamageDealt"; UpdateUnitStats(); }
    public void ChangeStatToDamageTaken() { currentStat = "DamageTaken"; UpdateUnitStats(); }
    public void ChangeStatToHealing() { currentStat = "Healing"; UpdateUnitStats(); }
}
