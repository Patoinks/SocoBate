using System.Collections;
using UnityEngine;
using Models;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;


public class UIFight : MonoBehaviour
{

    public Transform dapPosition1; // Assign in inspector
    public Transform dapPosition2; // Assign in inspector
    public SquadManager squadManager;
    public IEnumerator MoveUnitCloser(BaseUnit target, BaseUnit attacker, float duration)
    {
        yield return StartCoroutine(MoveUnitCloserCoroutine(target, attacker, duration));
    }
    private IEnumerator MoveUnitCloserCoroutine(BaseUnit target, BaseUnit attacker, float duration)
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


                yield return StartCoroutine(MoveUnit(attackerPrefab, originalPosition, attackPosition, duration));
                yield return new WaitForSeconds(duration);
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


    public IEnumerator MoveUnitToDapPosition(BaseUnit unit, bool left, float duration, bool rotation)
    {
        GameObject unitPrefab = FindUnitInHexContainer(unit);

        if (unitPrefab != null)
        {
            Vector3 originalPosition = unitPrefab.transform.position;
            Quaternion originalRotation = unitPrefab.transform.rotation;

            Transform dapPosition = left ? dapPosition1 : dapPosition2;
            Vector3 dapTarget = dapPosition.position;

            // Determine rotation: Only rotate the "right" unit left & "left" unit right
            Quaternion targetRotation = originalRotation;
            if ((rotation && !left) || (!rotation && left))
            {
                targetRotation *= Quaternion.Euler(0, 180f, 0); // Rotate 180°
            }

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                unitPrefab.transform.position = Vector3.Lerp(originalPosition, dapTarget, elapsedTime / duration);
                unitPrefab.transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            unitPrefab.transform.position = dapTarget;
            unitPrefab.transform.rotation = targetRotation;

            yield return new WaitForSeconds(duration); // Wait at dap position

            // Move back & reset rotation
            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                unitPrefab.transform.position = Vector3.Lerp(dapTarget, originalPosition, elapsedTime / duration);
                unitPrefab.transform.rotation = Quaternion.Slerp(targetRotation, originalRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            unitPrefab.transform.position = originalPosition;
            unitPrefab.transform.rotation = originalRotation;
        }
        else
        {
            Debug.LogError($"Unit prefab for {unit.unitName} not found!");
        }
    }








    public GameObject FindUnitInHexContainer(BaseUnit unit)
    {
        string unitIdentifier = unit.unitName + (squadManager.playerUnits.Contains(unit) ? "_Player" : "_Enemy");

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

    public IEnumerator RotateUnit(BaseUnit unit, float duration)
    {
        Debug.Log("Rotating unit.");

        // Find the unit in its hex first
        GameObject targetPrefab = FindUnitInHexContainer(unit);
        if (targetPrefab == null)
        {
            Debug.LogError("Unit not found in hex.");
            yield break; // Exit if the unit is not found
        }

        float elapsedTime = 0f;
        float totalRotations = 5f; // Total rotations (5 full turns)

        // Store the initial rotation
        Quaternion startingRotation = targetPrefab.transform.rotation;

        // The target rotation after 5 full turns (360 degrees * 5 turns)
        Quaternion targetRotation = startingRotation * Quaternion.Euler(0, 360 * totalRotations, 0);

        while (elapsedTime < duration)
        {
            // Calculate the current rotation step
            float t = elapsedTime / duration;
            targetPrefab.transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is exactly the target rotation
        targetPrefab.transform.rotation = targetRotation;
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
