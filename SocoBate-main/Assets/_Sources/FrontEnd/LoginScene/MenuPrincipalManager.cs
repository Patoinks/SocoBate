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
using TMPro;
using System.IO;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] public InputField nomeUsuarioInput;
    [SerializeField] public InputField senhaInput;
    [SerializeField] public GameObject passvalid;
    [SerializeField] public TextMeshProUGUI errorText;

    private string filePath;

    private async void Start()
    {
        passvalid.SetActive(false);

        // Set file path to the persistent data path (app-specific storage location)
        filePath = Path.Combine(Application.persistentDataPath, "lastLogin.txt");

        // Check if the file exists and automatically call OnLoginButtonClick if it does
        if (File.Exists(filePath))
        {
            Debug.Log("Last login file found. Auto-login...");

            // Read the content of the file
            string fileContent = File.ReadAllText(filePath);
            string[] lines = fileContent.Split('\n');

            // Ensure there are at least 2 lines (username and password)
            if (lines.Length >= 2)
            {
                string username = lines[0].Replace("Username: ", "").Trim();
                string password = lines[1].Replace("Password: ", "").Trim();

                // Set the InputFields with the saved username and password
                nomeUsuarioInput.text = username;
                senhaInput.text = password;

                // Proceed with login using the extracted username and password
                DisableAllButtons();
                try
                {
                    // Call the Login function from AccountController
                    Guid userId = await AccountController.Login(username, password);

                    if (userId != Guid.Empty)
                    {
                        Debug.Log("[LOGIN] Login successful. User ID: " + userId);

                        // Save the login details after successful login
                        SaveLastLogin(username, password);

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
                        errorText.text = "[LOGIN] Invalid username or password.";
                        EnableAllButtons();
                    }
                }
                catch (Exception ex)
                {
                    errorText.text = ex.Message;
                    EnableAllButtons();
                }
            }
            else
            {
                Debug.Log("[LOGIN] Invalid file content.");
            }
        }
    }

    private void SaveLastLogin(string username, string password)
    {
        // Save username and password to file in persistent data path
        string fileContent = $"Username: {username}\nPassword: {password}";
        File.WriteAllText(filePath, fileContent);
        Debug.Log("Last login saved at: " + filePath);
    }


    public void OnStartRegisterPassword()
    {
        passvalid.SetActive(true);
    }


    public async void OnLoginButtonClick()
    {
        string username = nomeUsuarioInput.text;
        string password = senhaInput.text;
        DisableAllButtons();
        try
        {
            // Call the Login function from AccountController
            Guid userId = await AccountController.Login(username, password);

            if (userId != Guid.Empty)
            {
                Debug.Log("[LOGIN] Login successful. User ID: " + userId);

                SaveLastLogin(username, password);


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
                errorText.text = "[LOGIN] Invalid username or password.";
                EnableAllButtons();
            }
        }
        catch (Exception ex)
        {
            errorText.text = ex.Message;
            EnableAllButtons();
        }
    }


    void DisableAllButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(); // Find all buttons in the scene
        foreach (Button btn in buttons)
        {
            btn.interactable = false; // Disable them
        }
    }

    void EnableAllButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(); // Find all buttons again
        foreach (Button btn in buttons)
        {
            btn.interactable = true; // Re-enable them
        }
    }
    public async void OnSqueaky()
    {
        DisableAllButtons();
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
        //SceneManager.LoadScene("TestsScene");
    }

    public async void OnRegisterButtonClick()
    {
        string username = nomeUsuarioInput.text;
        string password = senhaInput.text;

        if (Validator.ValidatePassword(password) == true)
        {
            try
            {
                // Call the Register function from AccountController
                await AccountController.Register(username, password);

                if (Context.UserContext.account.AccountId != Guid.Empty)
                {
                    Debug.Log("[REGISTER] Registration successful. User ID: " + Context.UserContext.account.AccountId);
                    // Proceed to next menu or do other actions upon successful registration
                    NicknameManager.Instance.AskForNickname();
                    SaveLastLogin(username, password);


                }
            }
            catch (Exception ex)
            {
                errorText.text = "[REGISTER] " + ex.Message;
            }
        }
        else
        {
            errorText.text = "Invalid Password Format";
        }
    }
}