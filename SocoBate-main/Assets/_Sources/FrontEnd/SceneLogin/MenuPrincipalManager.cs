using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.SceneManagement;
using Database;
using Context;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] public InputField nomeUsuarioInput;
    [SerializeField] public InputField senhaInput;
    [SerializeField] public GameObject passvalid;



    private void Start()
    {

        passvalid.SetActive(false);

    }

    public void OnStartRegisterPassword()
    {
        passvalid.SetActive(true);
    }


    public async void OnLoginButtonClick()
    {
        string username = nomeUsuarioInput.text;
        string password = senhaInput.text;

        try
        {
            // Call the Login function from AuthController
            Guid userId = await AccountController.Login(username, password);

            if (userId != Guid.Empty)
            {
                Debug.Log("[LOGIN] Login successful. User ID: " + userId);
                Context.UserContext.account.AccountId = userId;
                // Proceed to next menu or do other actions upon successful login
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentSceneIndex + 1);

            }
            else
            {
                Debug.Log("[LOGIN] Invalid username or password.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[LOGIN] Login error: " + ex.Message);
        }
    }

    public async void OnSqueaky()
    {
        string username = "Patoinks";
        string password = "#Martim123";

        try
        {
            // Call the Login function from AuthController
            Guid userId = await AccountController.Login(username, password);

            if (userId != Guid.Empty)
            {
                Debug.Log("[LOGIN] Login successful. User ID: " + userId);
                await FriendshipController.LoadFriends(userId);
                Context.UnitContext.LoadAllUnitsFromSerializedData();
                await UnitController.GetOwnedUnits(userId);
                // Proceed to next menu or do other actions upon successful login
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentSceneIndex + 1);

            }
            else
            {
                Debug.Log("[LOGIN] Invalid username or password.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[LOGIN] Login error: " + ex.Message);
        }



    }


    
    public void OnSqueakyFast()
    {
        SceneManager.LoadScene("SceneMenuMain");   
    }

    public async void OnRegisterButtonClick()
    {
        string username = nomeUsuarioInput.text;
        string password = senhaInput.text;

        try
        {
            // Call the Register function from AccountController
            await AccountController.Register(username, password);

            if (Context.UserContext.account.AccountId != Guid.Empty)
            {
                Debug.Log("[REGISTER] Registration successful. User ID: " + Context.UserContext.account.AccountId);
                // Proceed to next menu or do other actions upon successful registration
                NicknameManager.Instance.AskForNickname();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[REGISTER] Registration error: " + ex.Message);
        }
    }
}


