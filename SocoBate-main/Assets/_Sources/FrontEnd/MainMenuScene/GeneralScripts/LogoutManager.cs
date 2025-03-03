using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LogoutManager : MonoBehaviour
{
    private string filePath;

    private void Start()
    {
        // Set file path to the AppData or appropriate location
        filePath = Path.Combine(Application.persistentDataPath, "lastLogin.txt");
    }

    public void Logout()
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Delete the file
            File.Delete(filePath);
            Debug.Log("Last login file deleted successfully.");
        }
        else
        {
            Debug.Log("No last login file found.");
        }

        // Optionally, reset any UI elements or scene
        // For example, returning to the login screen:
        SceneManager.LoadScene("LoginScene"); // Replace "LoginScene" with your actual login scene name
    }
}
