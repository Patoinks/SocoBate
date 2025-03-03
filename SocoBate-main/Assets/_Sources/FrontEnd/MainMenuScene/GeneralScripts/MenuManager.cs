using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private LogoutManager logoutManager;  // Reference to the LogoutManager
    public TMPro.TMP_Text nicknameText;

    void Start()
    {
      nicknameText.text = Context.UserContext.account.Nickname;
    }


    public void OnLogoutButtonClick()
    {
        if (logoutManager != null)
        {
            logoutManager.Logout();  // Call the Logout method to delete the file and reset
        }
    }
}


