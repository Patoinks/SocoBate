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
    }

    public async void OnClickRefreshButton()
    {
        await Database.FriendshipController.GetFriends(Context.UserContext.account.AccountId);
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
            TMP_Text nameText = row.transform.Find("NameText").GetComponent<TMP_Text>();
            TMP_Text arenaEloText = row.transform.Find("ArenaEloText").GetComponent<TMP_Text>();
            TMP_Text onlineStatusText = row.transform.Find("OnlineStatusText").GetComponent<TMP_Text>();
            TMP_Text powerText = row.transform.Find("PowerText").GetComponent<TMP_Text>();
            Button duelButton = row.transform.Find("DuelButton").GetComponent<Button>();

            // Set data for the UI elements
            nameText.text = account.Nickname;
            arenaEloText.text = account.ArenaElo.ToString();
            onlineStatusText.text = "Online"; // You may want to check account.IsOnline
            powerText.text = "0";

            // Create a local copy of the account variable
            Account localAccount = account;
            duelButton.onClick.AddListener(() => onClickDuelButton(localAccount));
        }
    }

    void onClickDuelButton(Account account)
    {

        Debug.Log("Duel button clicked");
        Debug.Log("Duel against: " + account.Nickname);
        Context.DuelData.OpponentId = account.AccountId;
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
