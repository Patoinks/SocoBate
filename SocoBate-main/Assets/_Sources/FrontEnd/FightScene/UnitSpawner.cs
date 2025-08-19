// UnitSpawner.cs
using System.Collections.Generic;
using UnityEngine;
using Context;
using Models;

public class UnitSpawner : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameObject[] playerHexes;
    [SerializeField] private GameObject[] enemyHexes;

    // --- THIS IS THE FIX ---
    [Header("Prefab References")]
    // This field allows you to drag your "HealthBar2" prefab into the Inspector,
    // which is safer than loading by name.
    [SerializeField] private GameObject healthBarPrefab; 

    private readonly Dictionary<int, Transform> _playerHexPositions = new Dictionary<int, Transform>();
    private readonly Dictionary<int, Transform> _enemyHexPositions = new Dictionary<int, Transform>();

    void Start()
    {
        // Add a safety check to ensure the prefab is assigned.
        if (healthBarPrefab == null)
        {
            Debug.LogError("FATAL ERROR: The Health Bar Prefab is not assigned in the UnitSpawner's Inspector!");
            return;
        }

        InitializeHexPositions();
        SpawnSquadFromTeamSetup(TeamContext.GetPlayerTeam(), _playerHexPositions, false);
        SpawnSquadFromTeamSetup(TeamContext.GetEnemyTeam(), _enemyHexPositions, true);
    }

    private void InitializeHexPositions()
    {
        for (int i = 0; i < playerHexes.Length; i++) _playerHexPositions[i + 1] = playerHexes[i].transform;
        for (int i = 0; i < enemyHexes.Length; i++) _enemyHexPositions[i + 1] = enemyHexes[i].transform;
    }

    private void SpawnSquadFromTeamSetup(List<TeamSetup> team, IReadOnlyDictionary<int, Transform> hexPositions, bool isEnemy)
    {
        foreach (var unitSetup in team)
        {
            if (!hexPositions.ContainsKey(unitSetup.HexId)) continue;

            BaseUnit unitData = LoadUnitData(unitSetup.UnitName, unitSetup.HexId);
            GameObject unitPrefab = LoadUnitPrefab(unitSetup.UnitName);
            if (unitData == null || unitPrefab == null) continue;

            Transform hexTransform = hexPositions[unitSetup.HexId];

            GameObject spawnedUnitGO = Instantiate(unitPrefab, hexTransform);
            GameObject healthBarGO = Instantiate(healthBarPrefab, hexTransform);
            
            string uniqueID = $"{unitSetup.UnitName}_{(isEnemy ? "Enemy" : "Player")}";
            spawnedUnitGO.name = uniqueID;

            UnitFacade facade = spawnedUnitGO.GetComponent<UnitFacade>();
            if (facade == null) { Destroy(spawnedUnitGO); continue; }
            
            facade.Initialize(uniqueID, unitData, healthBarGO.GetComponent<HealthBar>(), isEnemy);
            UnitRegistry.Instance.RegisterUnit(uniqueID, facade);

            if (isEnemy)
            {
                spawnedUnitGO.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                Vector3 unitLocalPos = spawnedUnitGO.transform.localPosition;
                spawnedUnitGO.transform.localPosition = new Vector3(-unitLocalPos.x, unitLocalPos.y, unitLocalPos.z);
                Vector3 healthBarLocalPos = healthBarGO.transform.localPosition;
                healthBarGO.transform.localPosition = new Vector3(-healthBarLocalPos.x, healthBarLocalPos.y, healthBarLocalPos.z);
            }
        }
    }
    
    private GameObject LoadUnitPrefab(string unitName)
    {
        return Resources.Load<GameObject>($"UnitsPrefabs/{unitName}");
    }

    private BaseUnit LoadUnitData(string unitName, int hexId)
    {
        BaseUnit originalData = Resources.Load<BaseUnit>($"Units/UnitData/{unitName}");
        if (originalData == null) return null;
        BaseUnit clonedData = originalData.Clone();
        clonedData.HexId = hexId;
        clonedData.name = originalData.name;
        return clonedData;
    }
}