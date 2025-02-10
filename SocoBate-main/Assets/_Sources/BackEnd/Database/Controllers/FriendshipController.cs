using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MySql.Data.MySqlClient;
using Context;
using Models;

namespace Database
{
    public static class FriendshipController
    {
        // Method to check if a friendship exists between two users
        public async static Task<bool> IsFriend(Guid accountId1, Guid accountId2)
        {
            string query = @"
                SELECT 1 FROM Friendship
                WHERE (account_id_1 = @accountId1 AND account_id_2 = @accountId2)
                OR (account_id_1 = @accountId2 AND account_id_2 = @accountId1)
                LIMIT 1";

            var parameters = new Dictionary<string, object>
            {
                { "@accountId1", accountId1 },
                { "@accountId2", accountId2 }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            return result?.Count > 0; // Return true if friendship exists
        }

        // Method to add a friendship between two users
        public async static Task<bool> AddFriend(Guid accountId1, Guid accountId2)
        {
            // Step 1: Check if the friendship already exists
            if (await IsFriend(accountId1, accountId2))
            {
                Debug.Log("Friendship already exists.");
                return false; // Friendship already exists, can't proceed
            }

            // Step 2: Insert the new friendship into the database
            string query = @"
                INSERT INTO Friendship (account_id_1, account_id_2)
                VALUES (@accountId1, @accountId2)";

            var parameters = new Dictionary<string, object>
            {
                { "@accountId1", accountId1 },
                { "@accountId2", accountId2 }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            if (result != null) // Assuming non-null result means successful execution
            {
                Debug.Log("Friendship added successfully.");
                return true;
            }

            Debug.LogError("Error adding friendship.");
            return false; // There was an error while adding friendship
        }

        // Method to remove a friendship between two users
        public async static Task<bool> RemoveFriend(Guid accountId1, Guid accountId2)
        {
            string query = @"
                DELETE FROM Friendship
                WHERE (account_id_1 = @accountId1 AND account_id_2 = @accountId2)
                OR (account_id_1 = @accountId2 AND account_id_2 = @accountId1)";

            var parameters = new Dictionary<string, object>
            {
                { "@accountId1", accountId1 },
                { "@accountId2", accountId2 }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            if (result != null && result.Count > 0)
            {
                Debug.Log("Friendship removed successfully.");
                return true;
            }

            Debug.LogError("Error removing friendship.");
            return false; // There was an error while removing friendship
        }

        // Method to get a list of friends for a given account
        public async static Task<List<Guid>> GetFriends(Guid accountId)
        {
            string query = @"
                SELECT account_id_1, account_id_2
                FROM Friendship
                WHERE account_id_1 = @accountId OR account_id_2 = @accountId";

            var parameters = new Dictionary<string, object>
            {
                { "@accountId", accountId }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            List<Guid> friends = new List<Guid>();

            if (result != null)
            {
                foreach (var row in result)
                {
                    Guid friend1 = Guid.Parse(Convert.ToString(row[0]));
                    Guid friend2 = Guid.Parse(Convert.ToString(row[1]));

                    // Add the friend IDs to the list (avoiding the account itself)
                    if (friend1 != accountId) friends.Add(friend1);
                    if (friend2 != accountId) friends.Add(friend2);
                }
            }

            return friends;
        }

        // Method to get the account ID of a user by their nickname
        public static async Task<Guid?> GetAccountIdByNickname(string nickname)
        {
            string query = @"
                SELECT account_id
                FROM Account
                WHERE nickname = @nickname
                LIMIT 1";

            var parameters = new Dictionary<string, object>
            {
                { "@nickname", nickname }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            if (result?.Count == 1)
            {
                return Guid.Parse(Convert.ToString(result[0][0]));
            }

            Debug.LogError($"Account with nickname '{nickname}' not found.");
            return null;
        }

        // Example of setting the friends in UserContext
        public static async Task LoadFriends(Guid userId)
        {
            // Query to get the friends of the user (no status filter)
            string query = @"
    SELECT a.account_id, a.username, a.email, a.password_hash, a.gems, a.tickets, a.level, a.experience, a.banned, a.arena_elo, a.nickname
    FROM Account a
    INNER JOIN Friendship f ON (f.account_id_1 = @userId AND f.account_id_2 = a.account_id) 
                            OR (f.account_id_2 = @userId AND f.account_id_1 = a.account_id)";

            var parameters = new Dictionary<string, object>
    {
        { "@userId", userId }
    };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            // Create a list of Account objects for the friends using FromRow method
            List<Account> friendsList = new List<Account>();

            foreach (var row in result)
            {
                // Use FromRow method to map the row to an Account object
                Account friend = Account.FromRow(row);
                friendsList.Add(friend);
            }

            // Set the friends list in the UserContext
            UserContext.SetFriends(friendsList);
        }


    }
}
