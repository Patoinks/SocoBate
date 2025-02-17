using UnityEngine;

public class ScrollViewManager : MonoBehaviour
{
    public GameObject ownedUnitRowPrefab;  // Prefab for the row (containing buttons, text, etc.)
    public Transform scrollViewContent;  // The container for the rows inside the ScrollView
    public TeamManager teamManager;  // Reference to TeamManager
    
    void Start()
    {
        // Ensure the TeamManager is properly assigned
        if (teamManager == null)
        {
            teamManager = FindObjectOfType<TeamManager>();
        }
    }

    // This function instantiates a row for the unit in the scroll view.
    public void SpawnOwnedUnitRow()
    {
        if (ownedUnitRowPrefab != null && scrollViewContent != null)
        {
            // Instantiate a complex row (not just a button) inside the scroll view content
            GameObject ownedUnitRow = Instantiate(ownedUnitRowPrefab, scrollViewContent);
            Debug.Log("Owned Unit Row instantiated: " + ownedUnitRow.name);

            // Optionally, add any setup for this row (e.g., assign data to Text or Image components)
            // You can populate text, images, or assign listeners here as needed.

        }
        else
        {
            Debug.LogError("Missing references in ScrollViewManager.");
        }
    }
}
