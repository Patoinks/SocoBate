using System.Collections;
using UnityEngine;
using Models;

public class UIFight : MonoBehaviour
{
    public SquadManager squadManager;
    public IEnumerator MoveUnitCloser(BaseUnit target, BaseUnit attacker)
    {
        yield return StartCoroutine(MoveUnitCloserCoroutine(target, attacker));
    }
    private IEnumerator MoveUnitCloserCoroutine(BaseUnit target, BaseUnit attacker)
    {
        GameObject targetPrefab = FindUnitInHexContainer(target);
        GameObject attackerPrefab = FindUnitInHexContainer(attacker);

        if (targetPrefab != null && attackerPrefab != null)
        {
            string targetHexName = targetPrefab.transform.parent != null ? targetPrefab.transform.parent.name : "";
            string attackerHexName = attackerPrefab.transform.parent != null ? attackerPrefab.transform.parent.name : "";

            if (IsOppositeHex(targetHexName, attackerHexName))
            {
                Vector3 attackPosition = targetPrefab.transform.position;
                Vector3 originalPosition = attackerPrefab.transform.position;
                float duration = 0.2f;

                yield return StartCoroutine(MoveUnit(attackerPrefab, originalPosition, attackPosition, duration));
                yield return new WaitForSeconds(0.1f);
                yield return StartCoroutine(MoveUnit(attackerPrefab, attackPosition, originalPosition, duration));
            }
            else
            {
                Debug.LogError("Units are not in opposite hexes. Movement not allowed.");
            }
        }
        else
        {
            Debug.LogError("Unit prefabs not found.");
        }
    }

    private IEnumerator MoveUnit(GameObject unit, Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            unit.transform.position = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        unit.transform.position = end;
    }

    public GameObject FindUnitInHexContainer(BaseUnit unit)
    {
        string unitIdentifier = unit.unitName + (squadManager.playerUnits.Contains(unit) ? "_Player" : "_Enemy");
        Debug.Log($"Looking for unit with identifier: {unitIdentifier}");

        for (int i = 1; i <= 9; i++)
        {
            foreach (string hexName in new[] { "Hex" + i, "Hex" + i + "J2" })
            {
                GameObject hex = GameObject.Find(hexName);
                if (hex != null)
                {

                    foreach (Transform child in hex.transform)
                    {
                        if (child.name == unitIdentifier)
                        {
                            return child.gameObject;
                        }
                    }
                }
            }
        }

        return null;
    }




    private GameObject FindUnitInHex(GameObject hex, BaseUnit unit)
    {
        foreach (Transform child in hex.transform)
        {
            BaseUnit unitComponent = child.GetComponent<BaseUnit>();
            if (unitComponent != null && unitComponent == unit) // Ensure exact match
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private bool IsOppositeHex(string attackerHex, string targetHex)
    {
        // Check if the hexes are opposites based on naming convention
        if (attackerHex.EndsWith("J2") && targetHex.StartsWith("Hex"))
        {
            return true;
        }
        else if (targetHex.EndsWith("J2") && attackerHex.StartsWith("Hex"))
        {
            return true;
        }
        return false;
    }
}
