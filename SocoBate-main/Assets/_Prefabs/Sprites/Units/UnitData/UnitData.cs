using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Units/UnitData", order = 1)]
public class UnitData : ScriptableObject
{
    public string unitName;
    public Sprite unitSprite;
    public int baseHp;
    public int baseAtk;
    public int baseDef;
    public int baseSpeed;
    public int baseStr;
    public int baseInt;
    public int rarity;
    
    // Add any other properties for this unit here
    
    // Passive Ability Info
    public string passiveEffect;
    public int passiveValue;
    
    // Special Attack Info
    public string specialEffect;
    public int specialValue;
}
