using System;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using UnityEngine;

public class DatabaseConnectorHook : MonoBehaviour
{
    // Asynchronous connection test
    public async void ConnectionTest()
    {
        await Task.Run(async () => 
        {
            int result = await Database.DatabaseConnector.PingDatabaseAsync(3);
        });
    }

    // Asynchronous query test
    public async void AsyncQueryTest()
    {
        await Task.Run(async () => 
        {
            List<object[]> result = await Database.DatabaseConnector.QueryDatabaseAsync("Select * from Account");
        });
    }

    public async void AsyncLoginTest()
    {
        await Task.Run(async () =>
        {
            Guid uid = await Database.AccountController.Login("test", "test");
        });
    }

    public async void AsyncRegisterTest()
    {
        await Task.Run(async () =>
        {
            await Database.AccountController.Register("test", "test");
        });
    }
}
