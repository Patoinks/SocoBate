using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnClickClose()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
