using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MySql.Data.MySqlClient;
using Context;
using Models;

namespace Database
{
    public static class AccountController
    {
        // Login method: Validate credentials and store user data in UserContext
        public async static Task<Guid> Login(string username, string password)
        {
            string hashedInput = Cryptography.HashSHA512(password);

            string query = "SELECT * FROM Account WHERE username = @username AND password_hash = @password_hash LIMIT 1";
            var parameters = new Dictionary<string, object>
    {
        { "@username", username },
        { "@password_hash", hashedInput }
    };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            if (result?.Count == 1)
            {
                object[] userData = result[0];
                Guid accountId = Guid.Parse(Convert.ToString(userData[0]));
                Account account = new Account(
                    accountId,
                    Convert.ToString(userData[1]),
                    Convert.ToString(userData[2]),
                    Convert.ToString(userData[3]),
                    Convert.ToInt32(userData[4]),
                    Convert.ToInt32(userData[5]),
                    Convert.ToInt32(userData[6]),
                    Convert.ToInt32(userData[7]),
                    Convert.ToBoolean(userData[8]),
                    Convert.ToInt32(userData[9]),
                    Convert.ToString(userData[10])
                );

                UserContext.SetUser(account);
                return accountId;
            }

            return Guid.Empty;
        }

        public class RegisterResult
        {
            public bool Success { get; set; }
            public Guid AccountId { get; set; }
            public string ErrorMessage { get; set; }
        }

        // Register method: Register a new user, validate input, and store user data
        public async static Task<RegisterResult> Register(string username, string password, string email = null, string nickname = "", bool loginOnRegister = true)
        {
            if (!Validator.ValidatePassword(password))
                return new RegisterResult { Success = false, ErrorMessage = "Password rules not respected." };

            Guid accountId = Guid.NewGuid();
            string hashedPass = Cryptography.HashSHA512(password);

            string insertQuery = @"
        INSERT INTO Account (account_id, username, email, password_hash, gems, tickets, level, experience, banned, arena_elo, nickname)
        VALUES (@accountId, @username, @email, @password_hash, 0, 0, 1, 0, false, 1000, @nickname)";

            var insertParams = new Dictionary<string, object>
    {
        { "@accountId", accountId },
        { "@username", username },
        { "@email", email },
        { "@password_hash", hashedPass },
        { "@nickname", nickname }
    };

            // Execute INSERT query
            List<object[]> insertResult = await DatabaseConnector.QueryDatabaseAsync(insertQuery, insertParams);

            if (insertResult == null) // Check for errors
            {
                return new RegisterResult { Success = false, ErrorMessage = "Failed to insert user data." };
            }

            // Fetch the newly created account
            string selectQuery = "SELECT * FROM Account WHERE account_id = @accountId LIMIT 1";
            var selectParams = new Dictionary<string, object> { { "@accountId", accountId } };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(selectQuery, selectParams);

            if (result?.Count == 1)
            {
                object[] userData = result[0];
                Account account = new Account(
                    accountId,
                    Convert.ToString(userData[1]),
                    Convert.ToString(userData[2]),
                    Convert.ToString(userData[3]),
                    Convert.ToInt32(userData[4]),
                    Convert.ToInt32(userData[5]),
                    Convert.ToInt32(userData[6]),
                    Convert.ToInt32(userData[7]),
                    Convert.ToBoolean(userData[8]),
                    Convert.ToInt32(userData[9]),
                    Convert.ToString(userData[10])
                );

                UserContext.SetUser(account);
                return new RegisterResult { Success = true, AccountId = accountId };
            }

            return new RegisterResult { Success = false, ErrorMessage = "Error retrieving user data after registration." };
        }



        // Method to change/update nickname for a user
        public async static Task<bool> AddNicknameToAccount(Guid accountId, string nickname)
        {
            // Step 1: Check if the nickname is already taken
            string checkQuery = "SELECT 1 FROM Account WHERE nickname = @nickname LIMIT 1";
            var checkParams = new Dictionary<string, object> { { "@nickname", nickname } };

            List<object[]> existingNick = await DatabaseConnector.QueryDatabaseAsync(checkQuery, checkParams);
            if (existingNick?.Count > 0)
            {
                Debug.Log("Nickname already taken.");
                return false; // Nickname is already in use, can't proceed
            }

            Context.UserContext.SetNickname(nickname);
            // Step 2: Update the user's nickname in the database
            string updateQuery = "UPDATE Account SET nickname = @nickname WHERE account_id = @accountId";
            var updateParams = new Dictionary<string, object>
    {
        { "@nickname", nickname },
        { "@accountId", accountId }
    };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(updateQuery, updateParams);

            if (result != null) // Assuming non-null result means successful execution
            {
                Debug.Log("Nickname updated successfully.");
                return true;
            }

            Debug.LogError("Error updating nickname.");
            return false; // There was an error while updating
        }

    }
}
