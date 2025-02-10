using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CreateUnitFromSerialized : MonoBehaviour
{
    public BaseUnit chico;
    // Start is called before the first frame update
    async Task Start()
    {
        await Database.UnitController.InsertUnitWithAllData(chico);
    }

}
