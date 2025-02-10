using UnityEngine;

[CreateAssetMenu(fileName = "BaseUnits", menuName = "Units/UnitData/BaseUnits", order = 1)]
public class BaseUnit : ScriptableObject
{
    public string unitName;
    public Sprite unitSprite;
    public Sprite splashImage;

    public int baseHp;
    public int baseDef;
    public int baseSpeed;
    public int baseStr;
    public int baseInt;
    public int baseEvasion;
    public int baseLuck;
    public int rarity;

    // Normal Attack
    public string normalAttackEffectType;  // Damage, Heal, Buff, Debuff, etc.
    public string normalAttackTargetedStat;  // HP, Speed, Strength, etc.
    public string normalAttackScalingAttribute;  // Strength, Intelligence, Speed, etc.
    public int normalAttackScalingPercent;  // Percentage scaling (e.g., 20 for 20%)
    public int normalAttackValue;  // Flat effect value (e.g., damage, heal amount)
    public bool isNormalAttackAoE;
    public bool isNormalAttackPercentage;  
    public bool isNormalAttackCC;  
    public int normalAttackCCDuration;  
    public string normalAttackDescription;

    // Passive Ability
    public string passiveEffectType;  
    public string passiveTargetedStat;  
    public string passiveScalingAttribute;  
    public int passiveScalingPercent;  
    public int passiveValue;  
    public bool isPassiveAoE;  
    public bool isPassivePercentage;  
    public bool isPassiveCC;  
    public int passiveCCDuration;  
    public string passiveDescription;

    // Special Attack
    public string specialEffectType;  
    public string specialTargetedStat;  
    public string specialScalingAttribute;  
    public int specialScalingPercent;  
    public int specialValue;  
    public bool isSpecialAttackAoE;  
    public bool isSpecialAttackPercentage;  
    public bool isSpecialAttackCC;  
    public int specialAttackCCDuration;  
    public int turnsToSpecial;  
    public string specialDescription;
}
