using System.Collections;
using UnityEngine;
using Models;

public class UIFight : MonoBehaviour
{
    public IEnumerator MoveUnitCloser(BaseUnit target, BaseUnit attacker)
    {
        yield return StartCoroutine(MoveUnitCloserCoroutine(target, attacker));
    }

    private IEnumerator MoveUnitCloserCoroutine(BaseUnit target, BaseUnit attacker)
    {
        // Find units in their respective hexes
        GameObject targetPrefab = FindUnitInHexContainer(target.unitName, target);
        GameObject attackerPrefab = FindUnitInHexContainer(attacker.unitName, attacker);

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

    private GameObject FindUnitInHexContainer(string unitName, BaseUnit unit)
    {
        // Iterate through hexes by name (e.g., Hex1, Hex2, ..., Hex9, Hex1J2, Hex2J2, ..., Hex9J2)
        for (int i = 1; i <= 9; i++)
        {
            // Search for the standard Hex and HexJ2
            string hexName = "Hex" + i;
            GameObject hex = GameObject.Find(hexName);
            if (hex != null)
            {
                // Look for the unit by name inside this hex
                Transform unitInHex = hex.transform.Find(unitName);
                if (unitInHex != null)
                {
                    return unitInHex.gameObject;
                }

                // If not found by exact name, check if it’s a cloned unit (e.g., unitName + "(Clone)")
                Transform clonedUnitInHex = hex.transform.Find(unitName + "(Clone)");
                if (clonedUnitInHex != null)
                {
                    return clonedUnitInHex.gameObject;
                }
            }

            // Check Hex1J2, Hex2J2, ..., Hex9J2 as well
            string hexJ2Name = "Hex" + i + "J2";
            hex = GameObject.Find(hexJ2Name);
            if (hex != null)
            {
                // Look for the unit by name inside this hex
                Transform unitInHex = hex.transform.Find(unitName);
                if (unitInHex != null)
                {
                    return unitInHex.gameObject;
                }

                // If not found by exact name, check if it’s a cloned unit (e.g., unitName + "(Clone)")
                Transform clonedUnitInHex = hex.transform.Find(unitName + "(Clone)");
                if (clonedUnitInHex != null)
                {
                    return clonedUnitInHex.gameObject;
                }
            }
        }

        // If no unit is found in any hex, return null
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
