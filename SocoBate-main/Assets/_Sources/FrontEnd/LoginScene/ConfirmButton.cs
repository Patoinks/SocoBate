using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Context;
public class ConfirmButton : MonoBehaviour
{
    public GameObject nicknamePrefab;
    [SerializeField] public InputField nicknameInput;
    public async void OnClickConfirm()
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
