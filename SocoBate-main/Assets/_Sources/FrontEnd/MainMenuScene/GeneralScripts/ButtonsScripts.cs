using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ButtonsScripts : MonoBehaviour
{
    public GameObject inventoryMenuPrefab;

    public GameObject profileMenuPrefab;
    public GameObject amigosMenuPrefab; // Reference to the AmigosMenu prefab
    public GameObject teamSetupPrefab; // Reference to the TeamSetupPrefab prefab
    public GameObject heroListprefab; // Reference to the TeamSetupPrefab prefab
    public GameObject gachaMenuPrefab; // Reference to the TeamSetupPrefab prefab
    public GameObject mailBoxprefab; // Reference to the TeamSetupPrefab prefab
    void Start()
    {

    }
    public void OnClickAmigosButton()
    {
        // Find the "Tudo" GameObject in the scene hierarchy
        GameObject tudoObject = GameObject.Find("Canvas");

        // Check if the "Tudo" GameObject was found
        if (tudoObject != null)
        {
            // Get the transform of the "Tudo" GameObject
            Transform tudoTransform = tudoObject.transform;

            // Instantiate the AmigosMenu prefab under the "Tudo" GameObject's transform
            Instantiate(amigosMenuPrefab, tudoTransform);
        }
        else
        {
            Debug.LogError("Could not find 'Tudo' GameObject in the scene hierarchy.");
        }
    }

        public void OnClickProfileButton()
    {
        // Find the "Tudo" GameObject in the scene hierarchy
        GameObject tudoObject = GameObject.Find("Canvas");

        // Check if the "Tudo" GameObject was found
        if (tudoObject != null)
        {
            // Get the transform of the "Tudo" GameObject
            Transform tudoTransform = tudoObject.transform;

            // Instantiate the AmigosMenu prefab under the "Tudo" GameObject's transform
            Instantiate(profileMenuPrefab, tudoTransform);
        }
        else
        {
            Debug.LogError("Could not find 'Tudo' GameObject in the scene hierarchy.");
        }
    }

        public void OnClickInventoryButton()
    {
        // Find the "Tudo" GameObject in the scene hierarchy
        GameObject tudoObject = GameObject.Find("Canvas");

        // Check if the "Tudo" GameObject was found
        if (tudoObject != null)
        {
            // Get the transform of the "Tudo" GameObject
            Transform tudoTransform = tudoObject.transform;

            // Instantiate the AmigosMenu prefab under the "Tudo" GameObject's transform
            Instantiate(inventoryMenuPrefab, tudoTransform);
        }
        else
        {
            Debug.LogError("Could not find 'Tudo' GameObject in the scene hierarchy.");
        }
    }
    public void OnClickHeroesButton()
    {
        // Find the "Tudo" GameObject in the scene hierarchy
        GameObject tudoObject = GameObject.Find("Canvas");

        // Check if the "Tudo" GameObject was found
        if (tudoObject != null)
        {
            // Get the transform of the "Tudo" GameObject
            Transform tudoTransform = tudoObject.transform;

            // Instantiate the AmigosMenu prefab under the "Tudo" GameObject's transform
            Instantiate(heroListprefab, tudoTransform);
        }
        else
        {
            Debug.LogError("Could not find 'Tudo' GameObject in the scene hierarchy.");
        }
    }

    public void OnClickMailBox()
    {
        // Find the "Tudo" GameObject in the scene hierarchy
        GameObject tudoObject = GameObject.Find("Canvas");

        // Check if the "Tudo" GameObject was found
        if (tudoObject != null)
        {
            // Get the transform of the "Tudo" GameObject
            Transform tudoTransform = tudoObject.transform;

            // Instantiate the AmigosMenu prefab under the "Tudo" GameObject's transform
            Instantiate(mailBoxprefab, tudoTransform);
        }
        else
        {
            Debug.LogError("Could not find 'Tudo' GameObject in the scene hierarchy.");
        }
    }

    public void OnClickTeamButton()
    {
       SceneManager.LoadScene("TeamScene");
    }
    public void OnClickGachaButton()
    {
        SceneManager.LoadScene("GachaScene");
    }

    public void InstantiateAmigosMenu()
    {
        GameObject amigosMenuInstance = Instantiate(amigosMenuPrefab, transform.position, Quaternion.identity);

        // Get the CloseAmigosMenu component from the instantiated prefab
        CloseMenu closeAmigosMenu = amigosMenuInstance.GetComponent<CloseMenu>();

        // Set the AmigosMenu instance in the CloseAmigosMenu component
        if (closeAmigosMenu != null)
        {
            closeAmigosMenu.SetAmigosMenuInstance(amigosMenuInstance);
        }
        else
        {
            Debug.LogError("CloseAmigosMenu component not found in AmigosMenu prefab.");
        }
    }
}
