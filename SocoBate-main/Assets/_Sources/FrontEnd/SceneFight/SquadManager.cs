using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    // Hexes for Player and Enemy
    public GameObject[] playerHexes;  // Assign player hexes in Unity (Hex1 -> Hex9)
    public GameObject[] enemyHexes;   // Assign enemy hexes in Unity (Hex1 -> Hex9)

    // Unit prefabs for Player and Enemy
    public GameObject[] unitPrefabs;  // Player unit prefabs (Index 0 = UnitID 1, Index 1 = UnitID 2, etc.)
    public GameObject[] enemyUnitPrefabs; // Enemy unit prefabs (Index 0 = EnemyUnitID 1, Index 1 = EnemyUnitID 2, etc.)

    private Dictionary<int, Transform> playerHexPositions = new Dictionary<int, Transform>();
    private Dictionary<int, Transform> enemyHexPositions = new Dictionary<int, Transform>();

    // Player Squad Data (UnitID -> HexID)
    private Dictionary<int, int> playerSquad = new Dictionary<int, int>
    {
        { 2, 4 },  // Player UnitID 2 should spawn in Hex 4
        { 3, 7 },  // Player UnitID 3 should spawn in Hex 7
        { 1, 1 }   // Player UnitID 1 should spawn in Hex 1
    };

    // Enemy Squad Data (UnitID -> HexID)
    private Dictionary<int, int> enemySquad = new Dictionary<int, int>
    {
        { 1, 7 },  // Enemy UnitID 5 should spawn in Hex 6
        { 2, 8 },  // Enemy UnitID 6 should spawn in Hex 8
        { 3, 9 }   // Enemy UnitID 4 should spawn in Hex 9
    };

    void Start()
    {
        // Store player hex positions in a dictionary for quick lookup
        for (int i = 0; i < playerHexes.Length; i++)
        {
            playerHexPositions[i + 1] = playerHexes[i].transform; // Player hexes' IDs start from 1
        }

        // Store enemy hex positions in a dictionary for quick lookup
        for (int i = 0; i < enemyHexes.Length; i++)
        {
            enemyHexPositions[i + 1] = enemyHexes[i].transform; // Enemy hexes' IDs start from 1
        }

        // Spawn both player and enemy squads
        SpawnSquad(playerSquad, unitPrefabs, playerHexPositions, false); // false for player squad
        SpawnSquad(enemySquad, enemyUnitPrefabs, enemyHexPositions, true); // true for enemy squad
        
    }

    // Function to spawn squad units
    void SpawnSquad(Dictionary<int, int> squadData, GameObject[] unitPrefabs, Dictionary<int, Transform> hexPositions, bool isEnemy)
    {
        foreach (var entry in squadData)
        {
            int unitID = entry.Key;
            int hexID = entry.Value;

            // Determine whether we're spawning a player or enemy unit
            GameObject unitPrefab = isEnemy ? enemyUnitPrefabs[unitID - 1] : unitPrefabs[unitID - 1];
            Transform hexTransform = hexPositions[hexID];

            if (unitPrefab != null && hexTransform != null)
            {
                // Instantiate unit at the hex position
                GameObject unit = Instantiate(unitPrefab, hexTransform.position, Quaternion.identity, hexTransform);

                // Adjust the position of the unit (e.g., for height or shift)
                unit.transform.localPosition += new Vector3(-5f, 58f, 0f);

                // Rotate enemy units
                if (isEnemy)
                {
                    unit.transform.rotation = Quaternion.Euler(0f, -180f, 0f);  // Rotate enemy units to face the other way
                }

                Debug.Log($"Spawned {(isEnemy ? "Enemy" : "Player")} Unit {unitID} at Hex {hexID}");
            }
            else
            {
                Debug.LogError($"Invalid {(isEnemy ? "Enemy" : "Player")} UnitID {unitID} or HexID {hexID}");
            }
        }
    }
}
