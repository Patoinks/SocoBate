using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Models;
using TMPro;
using Context;
using Database;

public class SaveButton : MonoBehaviour
{
    public GameObject ownedUnitRowPrefab;
    public Transform scrollViewContent;
    public List<GameObject> hexes;
    private List<string> selectedUnitIds = new List<string>();

    void Start()
    {

    }


    public async void OnSaveClicked()
    {
        Debug.Log("OnSaveClicked called.");

        List<(int HexId, string UnitName)> teamSetup = new List<(int, string)>();

        for (int i = 0; i < hexes.Count; i++)
        {
            GameObject hex = hexes[i];
            Debug.Log($"Checking hex {i} with {hex.transform.childCount} children.");

            if (hex.transform.childCount > 0)
            {
                Transform child = hex.transform.GetChild(0);
                Debug.Log($"Found child in hex {i}: {child.name}");

                string unitName = child.name.Replace("(Clone)", "").Trim();
                Debug.Log($"Processed unit name: {unitName}");

                teamSetup.Add((i + 1, unitName));
            }
        }

        if (teamSetup.Count == 0)
        {
            Debug.LogError("No units placed on hexes. Nothing to save.");
            return;
        }

        Debug.Log("Deleting existing team setup...");

        bool deleteSuccess = await TeamController.DeleteTeam(Context.UserContext.account.AccountId);

        if (deleteSuccess)
        {
            Debug.Log("Existing team layout deleted successfully.");
        }
        else
        {
            Debug.LogError("Failed to delete existing team layout.");
            return;
        }

        Debug.Log("Saving new team to database...");

        bool saveSuccess = await TeamController.SaveTeam(Context.UserContext.account.AccountId, teamSetup);

        if (saveSuccess)
        {
            Debug.Log("Team layout successfully saved!");
        }
        else
        {
            Debug.LogError("Failed to save team layout.");
        }
        TeamToContext();
    }

    public async void TeamToContext()
    {
        List<(int HexId, string UnitName)> teamSetup = await TeamController.LoadTeam(Context.UserContext.account.AccountId); // Await async method

        if (teamSetup.Count > 0)
        {
            Debug.Log($"[LOGIN] Loaded {teamSetup.Count} team units.");

            // Convert to TeamSetup objects
            List<TeamSetup> teamList = new List<TeamSetup>();
            foreach (var (hexId, unitName) in teamSetup)
            {
                Debug.Log($"[LOGIN] Storing {unitName} at Hex {hexId}");
                teamList.Add(new TeamSetup(Context.UserContext.account.AccountId, hexId, unitName)); // Ensure you pass correct userId here
            }

            TeamContext.SetPlayerTeam(teamList); // Set the loaded team in the context
        }
        else
        {
            Debug.Log("[LOGIN] No team found for this user.");
        }
    }
}
