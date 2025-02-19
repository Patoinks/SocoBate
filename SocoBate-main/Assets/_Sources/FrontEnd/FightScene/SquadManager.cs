using System.Collections.Generic;
using UnityEngine;
using Context;
using Models;

public class SquadManager : MonoBehaviour
{
    // Hexes for Player and Enemy
    public GameObject[] playerHexes;  // Assign player hexes in Unity (Hex1 -> Hex9)
    public GameObject[] enemyHexes;   // Assign enemy hexes in Unity (Hex1 -> Hex9)

    private Dictionary<int, Transform> playerHexPositions = new Dictionary<int, Transform>();
    private Dictionary<int, Transform> enemyHexPositions = new Dictionary<int, Transform>();

    void Start()
    {
        if (playerHexes == null || playerHexes.Length == 0)
        {
            Debug.LogError("Player hexes are not assigned in the inspector.");
            return;
        }

        if (enemyHexes == null || enemyHexes.Length == 0)
        {
            Debug.LogError("Enemy hexes are not assigned in the inspector.");
            return;
        }

        // Store player hex positions
        for (int i = 0; i < playerHexes.Length; i++)
        {
            playerHexPositions[i + 1] = playerHexes[i].transform;
        }

        // Store enemy hex positions
        for (int i = 0; i < enemyHexes.Length; i++)
        {
            enemyHexPositions[i + 1] = enemyHexes[i].transform;
        }

        // Retrieve teams from TeamContext
        List<TeamSetup> playerTeam = TeamContext.GetPlayerTeam();
        List<TeamSetup> enemyTeam = TeamContext.GetEnemyTeam();

        if (playerTeam == null || playerTeam.Count == 0)
        {
            Debug.LogError("Player team is empty or null.");
            return;
        }

        if (enemyTeam == null || enemyTeam.Count == 0)
        {
            Debug.LogError("Enemy team is empty or null.");
            return;
        }

        Dictionary<string, int> playerSquad = ConvertTeamToDictionary(playerTeam);
        Dictionary<string, int> enemySquad = ConvertTeamToDictionary(enemyTeam);

        // Spawn both player and enemy squads
        SpawnSquad(playerSquad, playerHexPositions, false);
        SpawnSquad(enemySquad, enemyHexPositions, true);
    }

    // Convert List<TeamSetup> to Dictionary<string, int> (UnitName -> HexID)
    Dictionary<string, int> ConvertTeamToDictionary(List<TeamSetup> teamList)
    {
        Dictionary<string, int> squad = new Dictionary<string, int>();
        foreach (var unit in teamList)
        {
            if (!string.IsNullOrEmpty(unit.UnitName))
            {
                squad[unit.UnitName] = unit.HexId;
            }
            else
            {
                Debug.LogError("UnitName is null or empty in TeamSetup");
            }
        }
        return squad;
    }

    // Function to spawn squad units
    void SpawnSquad(Dictionary<string, int> squadData, Dictionary<int, Transform> hexPositions, bool isEnemy)
    {
        if (squadData.Count == 0)
        {
            Debug.LogError($"{(isEnemy ? "Enemy" : "Player")} squad is empty, nothing to spawn.");
            return;
        }

        foreach (var entry in squadData)
        {
            string unitName = entry.Key;
            int hexID = entry.Value;

            if (!hexPositions.ContainsKey(hexID))
            {
                Debug.LogError($"Invalid HexID {hexID} for unit {unitName}");
                continue;
            }

            // Find unit prefab by name (now only loading from Resources)
            GameObject unitPrefab = FindUnitPrefab(unitName);

            if (unitPrefab == null)
            {
                Debug.LogError($"No prefab found for unit {unitName}");
                continue;
            }

            Transform hexTransform = hexPositions[hexID];

            // Instantiate unit at the hex position
            GameObject unit = Instantiate(unitPrefab, hexTransform.position, Quaternion.identity, hexTransform);

            // Adjust the position of the unit (e.g., for height or shift)
            unit.transform.localPosition += new Vector3(-5f, 58f, 0f);

            // Rotate enemy units
            if (isEnemy)
            {
                unit.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            Debug.Log($"Spawned {(isEnemy ? "Enemy" : "Player")} Unit {unitName} at Hex {hexID}");
        }
    }

    // Function to find unit prefab by name, only loading from Resources/Units/Prefabs/
    GameObject FindUnitPrefab(string unitName)
    {
        string cleanedName = unitName.Replace(".prefab", "");

        // Load prefab from Resources/Units/Prefabs/
        GameObject loadedPrefab = Resources.Load<GameObject>($"UnitsPrefabs/{cleanedName}");

        if (loadedPrefab == null)
        {
            Debug.LogError($"Prefab '{cleanedName}' not found in Resources/UnitsPrefabs/.");
        }

        return loadedPrefab;
    }
}