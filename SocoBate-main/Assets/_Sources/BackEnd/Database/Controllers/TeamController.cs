using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MySql.Data.MySqlClient;
using Models;
using Context;

namespace Database
{
    public static class TeamController
    {
        // Save the team to the database (this will overwrite the current setup)
        public async static Task<bool> SaveTeam(Guid accountId, List<(int HexId, string UnitName)> teamSetup)
        {
            // Ensure the list is not empty before proceeding
            if (teamSetup == null || teamSetup.Count == 0)
            {
                Debug.LogError("Team setup is empty. Nothing to save.");
                return false;
            }

            // Start by removing the current team setup for the account
            string deleteQuery = "DELETE FROM TeamSetup WHERE AccountId = @accountId";
            var deleteParams = new Dictionary<string, object>
    {
        { "@accountId", accountId }
    };
            await DatabaseConnector.QueryDatabaseAsync(deleteQuery, deleteParams);
            Debug.Log($"Removed old team setup for AccountId: {accountId}");

            // Insert the new team setup
            string insertQuery = @"
        INSERT INTO TeamSetup (AccountId, HexId, UnitName)
        VALUES (@accountId, @hexId, @unitName)";

            // Loop through the team setup and insert each unit into the TeamSetup table
            foreach (var (hexId, unitName) in teamSetup)
            {
                // Debug: Log each insert operation
                Debug.Log($"Inserting Team Setup for AccountId: {accountId}, HexId: {hexId}, UnitName: {unitName}");

                var insertParams = new Dictionary<string, object>
        {
            { "@accountId", accountId },
            { "@hexId", hexId },
            { "@unitName", unitName }
        };
                await DatabaseConnector.QueryDatabaseAsync(insertQuery, insertParams);
            }

            Debug.Log("New team setup saved successfully.");
            return true;
        }


        // Load the team from the database for a given account
        public async static Task<List<(int HexId, string UnitName)>> LoadTeam(Guid accountId)
        {
            string query = "SELECT HexId, UnitName FROM TeamSetup WHERE AccountId = @accountId";
            var parameters = new Dictionary<string, object>
            {
                { "@accountId", accountId }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            List<(int HexId, string UnitName)> teamSetup = new List<(int, string)>();

            // Process the result
            if (result != null)
            {
                foreach (var row in result)
                {
                    int hexId = Convert.ToInt32(row[0]);
                    string unitName = Convert.ToString(row[1]);

                    teamSetup.Add((hexId, unitName));
                }
            }

            return teamSetup;
        }
    }
}
