using System;
using System.Collections.Generic;
using Models;

namespace Context
{
    public static class UserContext
    {
        // Store the current logged-in account data (including account_id)
        public static Account account { get; set; }

        // Store the list of friends for the current user
        public static List<Account> friends { get; set; } = new List<Account>(); // Empty list by default

        // Method to set the current user (store in UserContext)
        public static void SetUser(Account currentAccount)
        {
            account = currentAccount;
        }

        // Method to clear the current user (log out)
        public static void ClearUser()
        {
            account = null;
            friends.Clear(); // Also clear friends list when logging out
        }

        public static void ClearFriends()
        {
            friends.Clear(); // Also clear friends list when logging out
        }

        // Method to set the list of friends
        public static void SetFriends(List<Account> friendList)
        {
            friends = friendList;
        }

        // Method to retrieve the list of friends
        public static List<Account> GetFriends()
        {
            return friends;
        }

        // Method to get the nickname of the current user
        public static string GetNickname()
        {
            return account?.Nickname; // Ensure null safety
        }

        // Method to set the nickname of the current user
        public static void SetNickname(string nickname)
        {
            if (account != null)
            {
                account.Nickname = nickname;
            }
        }
    }
}
