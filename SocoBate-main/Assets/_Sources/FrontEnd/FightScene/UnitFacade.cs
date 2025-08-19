// UnitFacade.cs (Definitive, with Stats Panel Trigger)
using UnityEngine;

public class UnitFacade : MonoBehaviour
{
    public BaseUnit UnitData { get; private set; }
    public HealthBar HealthBar { get; private set; }
    public bool IsEnemy { get; private set; }
    public string UniqueID { get; private set; }

    public void Initialize(string uniqueID, BaseUnit unitData, HealthBar healthBar, bool isEnemy)
    {
        UniqueID = uniqueID;
        UnitData = unitData;
        HealthBar = healthBar;
        IsEnemy = isEnemy;
        
        if (HealthBar != null && UnitData != null)
        {
            HealthBar.SetMaxHealth(UnitData.maxHp);
            HealthBar.SetHealth(UnitData.baseHp);
        }
    }
}