using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using Database;
using Models;
using TMPro;

public class FriendCRUD : MonoBehaviour
{
    public TMP_InputField nicknameInputField;

    public FriendList friendList;

    public async void AddFriend()
    {
        string nickname = nicknameInputField.text;
        Guid? friendId = await FriendshipController.GetAccountIdByNickname(nickname);
        Guid userId = Context.UserContext.account.AccountId;

        // Check if the friendId is null
        if (friendId.HasValue)
        {
            // Check if the user is trying to add themselves
            if (friendId.Value == userId)
            {
                Debug.Log("You cannot add yourself as a friend.");
                return;
            }

            // Add the friend
            bool success = await FriendshipController.AddFriend(userId, friendId.Value);
            if (success)
            {
                Debug.Log($"Friend '{nickname}' added successfully.");
                friendList.OnClickRefreshButton();
            }
            else
            {
                Debug.Log($"Failed to add friend '{nickname}'.");
            }

            // Log the friend ID (since it is valid now)
            Debug.Log($"Friend ID for '{nickname}': {friendId.Value}");
        }
        else
        {
            Debug.Log($"No friend found with the nickname '{nickname}'.");
        }
    }

    public void AddFriendOnClick()
    {
        AddFriend();
    }
}
