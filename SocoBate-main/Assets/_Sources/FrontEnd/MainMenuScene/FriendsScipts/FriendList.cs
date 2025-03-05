using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using Database;
using Models;
using TMPro;
using System.Threading.Tasks;

public class FriendList : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject amigos; // This is your row panel prefab
    public Transform painelamigos; // This is the parent transform where the rows will be instantiated
    public int bufferSize = 10; // Number of rows to keep in memory above and below visible area

    void Start()
    {
        PopulateRow();
        Debug.Log("FriendList script started");
    }
    public async void OnClickRefreshButton()
    {
        // Clear existing rows
        foreach (Transform child in painelamigos)
        {
            Destroy(child.gameObject);
        }

        // Fetch new friend list from the database
        await Database.FriendshipController.GetFriends(Context.UserContext.account.AccountId);

        // Populate UI with updated data
        PopulateRow();
    }


    void PopulateRow()
    {
        // Use GetFriends() method to retrieve the friends list from UserContext
        List<Account> accounts = Context.UserContext.GetFriends();

        if (accounts.Count == 0)
        {
            Debug.Log("No friends found");
            return;
        }

        foreach (Account account in accounts)
        {
            GameObject row = Instantiate(amigos, painelamigos);

            // Get references to the UI elements within the panel
            TMP_Text nameText = row.transform.Find("Panel/NameText").GetComponent<TMP_Text>();
            TMP_Text onlineStatusText = row.transform.Find("Panel/OnlineStatusText").GetComponent<TMP_Text>();
            Button duelButton = row.transform.Find("Panel/DuelButton").GetComponent<Button>();

            // Set data for the UI elements
            nameText.text = account.Nickname;
            onlineStatusText.text = "Online"; // You may want to check account.IsOnline
            // Create a local copy of the account variable
            Account localAccount = account;
            duelButton.onClick.AddListener(() => onClickDuelButton(localAccount));
        }
    }

    async void onClickDuelButton(Account account)
    {
        Debug.Log("Duel button clicked");
        Debug.Log("Duel against: " + account.Nickname);

        // Start loading the enemy team asynchronously without waiting
        await TeamController.GetSquadByNickname(account.Nickname);  // No need to await, just call it asynchronously

        // Immediately load the FightScene
        SceneManager.LoadScene("FightScene");
    }



    void Update()
    {
        // Listen for the ESC key to destroy the current unit prefab
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Prefab != null)
            {
                Destroy(Prefab);  // Destroy the prefab
                Prefab = null;  // Reset the reference
            }
        }
    }
}
