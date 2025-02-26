using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BaseUnits", menuName = "Units/UnitData/BaseUnits", order = 1)]
public class BaseUnit : ScriptableObject
{
    public string unitName;
    public Sprite unitSprite;
    public Sprite splashImage;

    public int maxHp;
    public int baseHp;
    public int mDef;  // Magical Defense
    public int pDef;  // Physical Defense
    public int baseSpeed;
    public int baseStr;
    public int baseInt;
    public int baseEvasion;
    public int baseLuck;
    public int rarity;

    // Attack Data Structure
    [System.Serializable]
    public class AttackData
    {
        public List<Effect> effects; // Multiple effects per attack (Damage, Heal, Buff, Debuff, etc.)
        public string description;
        public int turnsToSpecial; // Only used for Special Attacks
    }

    // Effect Structure (Damage, Heal, Buff, Debuff)
    [System.Serializable]
    public class Effect
    {
        public EffectType effectType; // Damage, Heal, Buff, Debuff
        public TargetType targetType; // Who gets affected (moved inside Effect)
        public string targetedStat; // Single stat affected (e.g., HP, Speed, Strength)
        public string scalingAttribute; // Strength, Intelligence, etc.
        public int scalingPercent; // % Scaling (e.g., 20 for 20%)
        public int baseValue; // Flat effect value (e.g., damage, heal amount)
        public bool isPercentage; // True if it applies as a % instead of a flat value
        public StatusEffect statusEffect; // Optional CC effect
    }

    // Status Effect Structure
    [System.Serializable]
    public class StatusEffect
    {
        public CrowdControlType ccType; // Stun, Root, Silence, etc.
        public int duration; // Duration in turns
        public int tickDamage; // Damage per turn for effects like Poison
        public string scalingAttribute; // STR, INT, etc.
        public int scalingPercent; // Scaling for DoT effects (e.g., Poison scales off INT)
        public bool isPercentage; // If true, applies as % of target’s HP/Stat
        public bool preventsAction;
        public bool preventsMovement;
        public bool preventsAttacks;
    }

    // Types of Effects
    public enum EffectType
    {
        Damage, 
        Heal, 
        Buff, 
        Debuff
    }

    // Crowd Control Types
    public enum CrowdControlType
    {
        None,
        Stun,   // Prevents all actions
        Root,   // Prevents movement
        Silence, // Prevents casting abilities
        Blind,  // Reduces accuracy
        Slow,   // Reduces movement speed
        Poison, // Deals damage over time
        Burn,    // Deals damage over time
        Taunt
    }

    // Target Types (For Different Attack/Effect Applications)
    public enum TargetType
    {
        Self, // Only affects the caster
        SingleAlly, // Targets one ally
        AllAllies, // Affects all allies
        SingleEnemy, // Targets one enemy
        AllEnemies // Affects all enemies
    }

    // Attacks & Abilities
    public AttackData normalAttack;
    public AttackData passiveAbility;
    public AttackData specialAttack;
}
