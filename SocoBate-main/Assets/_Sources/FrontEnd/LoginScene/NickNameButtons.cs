using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Context;
using TMPro;
public class NickNameButtons : MonoBehaviour
{
    public GameObject nicknamePrefab;
    [SerializeField] public InputField nicknameInput;

    [SerializeField] public TextMeshProUGUI errorText;
    public async void OnClickConfirm()
    {
        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            errorText.text = "Nickname cannot be empty.";
            return;
        }
        else if (nicknameInput.text.Length < 3)
        {
            errorText.text = "Nickname must be at least 3 characters long.";
            return;
        }
        else if (nicknameInput.text.Length > 20)
        {
            errorText.text = "Nickname must be at most 20 characters long.";
            return;
        }
        else if (nicknameInput.text.Contains(" "))
        {
            errorText.text = "Nickname cannot contain spaces.";
            return;
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(nicknameInput.text, @"^[a-zA-Z0-9]+$"))
        {
            errorText.text = "Nickname can only contain letters and numbers.";
            return;
        }
        else
        {
            string nickname = nicknameInput.text;

            try
            {
                // Ensure user is logged in and the user data is available in UserContext
                if (Context.UserContext.account == null)
                {
                    Debug.LogError("No user is logged in.");
                    return; // Exit the method if the user is not logged in
                }

                // Now we can safely access Context.UserContext.user.id
                Guid userId = Context.UserContext.account.AccountId;

                // Call AddNicknameToAccount with the user ID and the new nickname
                bool successful = await AccountController.AddNicknameToAccount(userId, nickname);

                if (successful)
                {
                    // Load the main menu scene after successful nickname change
                    SceneManager.LoadScene("MainMenuScene");
                }
                else
                {
                    Debug.LogError("Failed to update nickname.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Nickname Register error: " + ex.Message);
            }
        }
    }
}

