using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Database; // Ensure this namespace is correct

public class CreateUnitFromSerialized : MonoBehaviour
{
    public List<BaseUnit> baseUnits; // List to store all units

    // Start is called before the first frame update
    async Task Start()
    {
        foreach (var unit in baseUnits)
        {
            if (unit != null) // Ensure the unit is not null before inserting
            {
                await UnitCreator.InsertOrUpdateUnitWithAllData(unit);
            }
        }
    }
}
