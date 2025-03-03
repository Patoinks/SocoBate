using UnityEngine;
using Context;
using System.Collections;

public class CloseMenu : MonoBehaviour
{
    // Reference to the AmigosMenu prefab instance
     [SerializeField]
    private GameObject amigosMenuInstance;

    // Method to set the AmigosMenu prefab instance
    void Start()
    {
        UserContext.GetFriends();
    }

    public void SetAmigosMenuInstance(GameObject instance)
    {
        amigosMenuInstance = instance;
    }

    // Method to close the AmigosMenu prefab instance
public void CloseMenuButton()
{
    // Check if the AmigosMenu prefab instance exists
    if (amigosMenuInstance != null)
    {
        // Destroy the prefab instance
        UserContext.ClearFriends();
        Destroy(amigosMenuInstance);
    }
    else
    {
        Debug.LogError("AmigosMenu instance not set.");
    }
}
}