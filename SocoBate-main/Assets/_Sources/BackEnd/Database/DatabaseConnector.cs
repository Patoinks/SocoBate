using System;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace Database
{
    public static class DatabaseConnector
    {
        public static string ipv4 = "13.39.219.186";
        public static string database = "AuraGacha";
        public static string username = "martim123";
        public static string password = "#Timtocas2003"; // encrypt?
        public static bool ExtensionOfNativeClass = true;

        public static string connectionString
        {
            get
            {
                return $"Server={ipv4};Database={database};Uid={username};Pwd={password};";
            }
        }

        // Asyncronously tries to connect to SQL server instance on EC2
        public static async Task<int> PingDatabaseAsync(int tries = 1)
        {
            int bitRes = 0;
            for (int i = 0; i < tries; i++)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        await connection.OpenAsync();
                        Debug.Log(tries == 1 ? $"ping {ipv4} success" : $"ping {ipv4} success, try: {i + 1}/{tries}");
                        bitRes = 1;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(tries == 1 ? $"ping {ipv4} error\n{ex.Message}" : $"ping {ipv4} error\n{ex.Message} try: {i + 1}/{tries}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            Debug.Log($"ping {ipv4} bitRes = {bitRes} tries = {tries}");
            return bitRes;
        }

        // Asynchronously queries the database
        public static async Task<List<object[]>> QueryDatabaseAsync(string query, Dictionary<string, object> parameters = null)
        {
            Debug.Log($"[DATABASE] Query - {query}");

            List<object[]> resultData = new List<object[]>();

            using MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();

                using MySqlCommand cmd = new MySqlCommand(query, connection);

                // Add parameters if provided
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                using DbDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    object[] rowData = new object[reader.FieldCount];
                    reader.GetValues(rowData);
                    resultData.Add(rowData);
                }

                return resultData;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DATABASE] Query - Error executing query asynchronously: {ex.Message}");
                return null;
            }
        }


        // Synchronously queries the database
        //// DO NOT USE UNLESS ABSOLUTELY NECESSARY
        public static DbDataReader QueryDatabase(string query)
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();

                using MySqlCommand cmd = new MySqlCommand(query, connection);
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Debug.Log($"[DATABASE] Error executing query synchronously: {ex.Message}");
                return null;
            }
        }
    }
}
