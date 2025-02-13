using UnityEngine;
using UnityEngine.UI;

public class HexButton : MonoBehaviour
{
    public GameObject hex;  // Reference to the actual hex GameObject
    private TeamManager teamManager;  // Reference to TeamManager
    private Button button;

    void Start()
    {
        // Find the TeamManager in the scene
        teamManager = FindObjectOfType<TeamManager>();

        // Get the Button component and add listener for when the hex is clicked
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnHexClicked);
            SetButtonInteractable(false);  // Disable hex button initially
        }
    }



    // Called when the hex button is clicked (left-click for spawning unit or removing)
    void OnHexClicked()
    {
        if (teamManager != null && hex != null)
        {
            // If hex already contains a unit, remove it
            if (hex.transform.childCount > 0)
            {
                teamManager.RemoveUnitFromHex(hex);  // Remove the unit
            }
            else
            {
                // If no unit is present, spawn a new unit
                teamManager.SpawnUnitOnHex(hex);  // Spawn a unit on this hex
            }

            // Disable the button after action is done (if desired, otherwise enable later)
            SetButtonInteractable(false);
        }
    }

    // Call this method to enable or disable hex interaction (button)
    public void SetButtonInteractable(bool interactable)
    {
        button.interactable = interactable;  // Enable or disable the button
    }
}
