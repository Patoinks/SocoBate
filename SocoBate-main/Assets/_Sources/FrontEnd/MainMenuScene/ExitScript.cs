using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Context;
using Database;
using Models;
public class ExitScript : MonoBehaviour
{
    public void OnClickClose() // Fix async method
    {
        SceneManager.LoadScene("MainMenuScene"); // Load the main menu
    }
}
