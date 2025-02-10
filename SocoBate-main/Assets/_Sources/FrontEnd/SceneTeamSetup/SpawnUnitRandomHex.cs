using UnityEngine;
using System.Collections.Generic;

public class UnitSelector : MonoBehaviour
{
    public GameObject unitPrefab;  // The unit prefab to spawn
    public List<GameObject> hexes;  // List of hexes where the unit can be spawned

    // Reference to the Canvas > Panel > Panel hierarchy
    public Transform parentPanel;  // Set this in the Inspector to Canvas > Panel > Panel

    // Function to spawn the unit at a random hex's position
    public void SpawnUnitInRandomHex()
    {
        // Choose a random hex from the list
        GameObject randomHex = hexes[Random.Range(0, hexes.Count)];

        // Ensure the hex has a transform
        if (randomHex != null && parentPanel != null)
        {
            // Get the position of the hex
            Vector3 hexPosition = randomHex.transform.position;

            // Instantiate the unit at the hex's position, but inside the Canvas > Panel > Panel hierarchy
            GameObject unit = Instantiate(unitPrefab, hexPosition, Quaternion.identity, parentPanel);

            // Optionally adjust the position (like in the SquadManager example)
            unit.transform.localPosition += new Vector3(-5f, 58f, 0f);  // Adjust as needed

            // Optional: Rotate the unit (if you want it facing a specific direction)
            unit.transform.rotation = Quaternion.Euler(0f, 0f, 0f);  // Adjust rotation as needed

            Debug.Log("Unit spawned at hex: " + randomHex.name);
        }
        else
        {
            Debug.LogError("Random hex is null or parentPanel is not set.");
        }
    }
}
