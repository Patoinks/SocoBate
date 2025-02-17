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
using Models;
using System.Collections.Generic;

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
            // Call the Login function from AccountController
            Guid userId = await AccountController.Login(username, password);

            if (userId != Guid.Empty)
            {
                Debug.Log("[LOGIN] Login successful. User ID: " + userId);

                // Load Friends
                await FriendshipController.LoadFriends(userId);

                // Load Units
                Context.UnitContext.LoadAllUnitsFromSerializedData();
                await UnitController.GetOwnedUnits(userId);

                // Load Team from Database and Store in Context
                List<(int HexId, string UnitName)> teamSetup = await TeamController.LoadTeam(userId);
                if (teamSetup.Count > 0)
                {
                    Debug.Log($"[LOGIN] Loaded {teamSetup.Count} team units.");

                    // Convert to TeamSetup objects
                    List<TeamSetup> teamList = new List<TeamSetup>();
                    foreach (var (hexId, unitName) in teamSetup)
                    {
                        Debug.Log($"[LOGIN] Storing {unitName} at Hex {hexId}");
                        teamList.Add(new TeamSetup(userId, hexId, unitName));
                    }

                    TeamContext.SetPlayerTeam(teamList);
                }
                else
                {
                    Debug.Log("[LOGIN] No team found for this user.");
                }

                // Switch Scene
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
        SceneManager.LoadScene("TestsScene");
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


