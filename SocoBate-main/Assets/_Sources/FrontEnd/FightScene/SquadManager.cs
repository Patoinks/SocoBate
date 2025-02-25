using System.Collections;
using UnityEngine;
using Context;
using System.Collections.Generic;
using Models;

public class SquadManager : MonoBehaviour
{
    public GameObject[] playerHexes;  // Assign in Unity Editor
    public GameObject[] enemyHexes;   // Assign in Unity Editor

    private Dictionary<int, Transform> playerHexPositions = new Dictionary<int, Transform>();
    private Dictionary<int, Transform> enemyHexPositions = new Dictionary<int, Transform>();

    public List<BaseUnit> playerUnits = new List<BaseUnit>(); // Add this to hold player units for the battle
    public List<BaseUnit> enemyUnits = new List<BaseUnit>();  // Add this to hold enemy units for the battle

    public HealthBar healthBar;
    private Dictionary<BaseUnit, GameObject> unitPrefabs = new Dictionary<BaseUnit, GameObject>(); // Store unit prefabs

    void Start()
    {
        InitializeHexPositions();
        SpawnSquad(TeamContext.GetPlayerTeam(), playerHexPositions, false);
        SpawnSquad(TeamContext.GetEnemyTeam(), enemyHexPositions, true);

        // Link the player and enemy units to the DuelScript (assuming the DuelScript is on a GameObject)
        var duelScript = FindObjectOfType<DuelScript>();
        if (duelScript != null)
        {
            duelScript.playerUnits = playerUnits;
            duelScript.enemyUnits = enemyUnits;
        }
        else
        {
            Debug.LogError("DuelScript not found in the scene.");
        }
    }

    private void InitializeHexPositions()
    {
        for (int i = 0; i < playerHexes.Length; i++)
        {
            playerHexPositions[i + 1] = playerHexes[i].transform;
        }
        for (int i = 0; i < enemyHexes.Length; i++)
        {
            enemyHexPositions[i + 1] = enemyHexes[i].transform;
        }
    }

    private void SpawnSquad(List<TeamSetup> team, Dictionary<int, Transform> hexPositions, bool isEnemy)
    {
        foreach (var unit in team)
        {
            if (!hexPositions.ContainsKey(unit.HexId))
            {
                Debug.LogError($"Invalid HexID {unit.HexId} for unit {unit.UnitName}");
                continue;
            }

            GameObject unitPrefab = FindUnitPrefab(unit.UnitName);
            if (unitPrefab == null)
            {
                Debug.LogError($"No prefab found for unit {unit.UnitName}");
                continue;
            }

            Transform hexTransform = hexPositions[unit.HexId];
            GameObject spawnedUnit = Instantiate(unitPrefab, hexTransform.position, Quaternion.identity, hexTransform);

            // Store the prefab in the dictionary for later use
            unitPrefabs[LoadBaseUnit(unit.UnitName)] = spawnedUnit;

            // Rotate enemy units
            if (isEnemy)
            {
                spawnedUnit.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
            }

            // Clone a fresh instance of the unit for this battle
            BaseUnit clonedUnit = LoadBaseUnit(unit.UnitName);
            if (clonedUnit != null)
            {
                if (isEnemy)
                {
                    healthBar.Initialize(clonedUnit.baseHp, spawnedUnit.transform);
                    enemyUnits.Add(clonedUnit);
                    Debug.Log($"Added enemy unit: {clonedUnit.name}");
                }
                else
                {
                    healthBar.Initialize(clonedUnit.baseHp, spawnedUnit.transform);
                    playerUnits.Add(clonedUnit);
                    Debug.Log($"Added player unit: {clonedUnit.name}");
                }
            }
        }

        // Log the unit counts after spawning
        Debug.Log("Player Units Count: " + playerUnits.Count);
        Debug.Log("Enemy Units Count: " + enemyUnits.Count);

        

    }

    private GameObject FindUnitPrefab(string unitName)
    {
        // Load prefab from Resources/UnitsPrefabs/
        GameObject loadedPrefab = Resources.Load<GameObject>($"UnitsPrefabs/{unitName}");

        if (loadedPrefab == null)
        {
            Debug.LogError($"Prefab '{unitName}' not found in Resources/UnitsPrefabs/.");
        }

        return loadedPrefab;
    }

    private BaseUnit LoadBaseUnit(string unitName)
    {
        // Load the original BaseUnit from Resources
        BaseUnit originalUnit = Resources.Load<BaseUnit>($"Units/UnitData/{unitName}");

        if (originalUnit == null)
        {
            Debug.LogError($"BaseUnit '{unitName}' not found in Resources/Units/UnitData/.");
            return null;
        }

        // Clone to ensure it's a fresh instance
        BaseUnit clonedUnit = Instantiate(originalUnit);

        return clonedUnit;
    }
}
