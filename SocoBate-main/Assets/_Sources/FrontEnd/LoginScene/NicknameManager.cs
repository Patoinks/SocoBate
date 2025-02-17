using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NicknameManager : MonoBehaviour
{
    // Singleton instance
    private static NicknameManager instance;

    // Public property to access the instance
    public static NicknameManager Instance
    {
        get { return instance; }
    }

    public GameObject nicknamePrefab; // Reference to the TeamSetupPrefab prefab

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Ensure there's only one instance of NicknameManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // Destroy the duplicate instance
            Destroy(gameObject);
        }
    }

    public void AskForNickname()
    {
        // Find the "Canvas" GameObject in the scene hierarchy
        GameObject canvasObject = GameObject.Find("Canvas");

        // Check if the "Canvas" GameObject was found
        if (canvasObject != null)
        {
            // Get the transform of the "Canvas" GameObject
            Transform canvasTransform = canvasObject.transform;

            // Instantiate the nicknamePrefab under the "Canvas" GameObject's transform
            Instantiate(nicknamePrefab, canvasTransform);
        }
        else
        {
            Debug.LogError("Could not find 'Canvas' GameObject in the scene hierarchy.");
        }
    }


}
