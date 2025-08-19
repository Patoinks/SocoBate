// BaseUnit.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "BaseUnits", menuName = "Units/UnitData/BaseUnits", order = 1)]
public class BaseUnit : ScriptableObject
{
    // All of your existing fields remain exactly the same.
    public int HexId { get; set; }

    public string unitName;
    public Sprite unitSprite;
    public Sprite splashImage;

    public int maxHp;
    public int baseHp;
    public int mDef;
    public int pDef;
    public int baseSpeed;
    public int baseStr;
    public int baseInt;
    public int baseEvasion;
    public int baseLuck;
    public int rarity;
    public int aura;

    public int DapAttemptChance;
    public int DapSuccessChance;

    [HideInInspector] public float damageDealt;
    [HideInInspector] public float damageTaken;
    [HideInInspector] public float healingDone;

    [System.Serializable]
    public class AttackData
    {
        public List<Effect> effects;
        public string description;
        public string attackName;
        public int turnsToSpecial;
    }

    [System.Serializable]
    public class Effect
    {
        public EffectType effectType;
        public TargetType targetType;
        public string targetedStat;
        public string scalingAttribute;
        public int scalingPercent;
        public int baseValue;
        public bool isPercentage;
        public bool isRng;
        public bool auraRngChance;
        public StatusEffect statusEffect;
    }

    [System.Serializable]
    public class StatusEffect
    {
        public CrowdControlType ccType;
        public int duration;
        public int tickDamage;
        public string scalingAttribute;
        public int scalingPercent;
        public bool isPercentage;
        public bool preventsAction;
        public bool preventsMovement;
        public bool preventsAttacks;
        public bool isRng;
        public float rngChance;
        public bool isSummon;
        public BaseUnit summonUnit;
    }

    public enum EffectType
    {
        Damage, Heal, Buff, Debuff, Steal, Odds,
    }

    public enum CrowdControlType
    {
        None, Stun, Root, Silence, Blind, Slow, Poison, Burn, Taunt, Summon
    }

    public enum TargetType
    {
        Self, SingleAlly, AllAllies, SingleEnemy, AllEnemies
    }

    public AttackData normalAttack;
    public AttackData passiveAbility;
    public AttackData specialAttack;

    [HideInInspector] public bool isStunned;
    [HideInInspector] public bool isTaunting;
    [HideInInspector] public bool isRooted;
    [HideInInspector] public bool isSilenced;
    [HideInInspector] public bool isPoisoned;
    [HideInInspector] public int tauntDuration;
    [HideInInspector] public int stunDuration;
    [HideInInspector] public int rootDuration;
    [HideInInspector] public int silenceDuration;
    [HideInInspector] public int poisonDuration;

    // --- NEW, SAFE CLONING METHOD ---
    public BaseUnit Clone()
    {
        // Step 1: Create a new instance of this ScriptableObject in memory. This is the safe way.
        var clone = CreateInstance<BaseUnit>();

        // Step 2: Manually copy all value types and complex types from 'this' (the original asset) to the clone.
        clone.unitName = this.unitName;
        clone.unitSprite = this.unitSprite;
        clone.splashImage = this.splashImage;
        clone.maxHp = this.maxHp;
        clone.baseHp = this.maxHp; // Start with full health
        clone.mDef = this.mDef;
        clone.pDef = this.pDef;
        clone.baseSpeed = this.baseSpeed;
        clone.baseStr = this.baseStr;
        clone.baseInt = this.baseInt;
        clone.baseEvasion = this.baseEvasion;
        clone.baseLuck = this.baseLuck;
        clone.rarity = this.rarity;
        clone.aura = this.aura;
        clone.DapAttemptChance = this.DapAttemptChance;
        clone.DapSuccessChance = this.DapSuccessChance;
        
        // Deep copy attack data to ensure lists are new instances
        clone.normalAttack = new AttackData { effects = new List<Effect>(this.normalAttack.effects), description = this.normalAttack.description, attackName = this.normalAttack.attackName, turnsToSpecial = this.normalAttack.turnsToSpecial };
        clone.specialAttack = new AttackData { effects = new List<Effect>(this.specialAttack.effects), description = this.specialAttack.description, attackName = this.specialAttack.attackName, turnsToSpecial = this.specialAttack.turnsToSpecial };
        clone.passiveAbility = new AttackData { effects = new List<Effect>(this.passiveAbility.effects), description = this.passiveAbility.description, attackName = this.passiveAbility.attackName, turnsToSpecial = this.passiveAbility.turnsToSpecial };

        return clone;
    }

    public void UpdateDamageDealt(float amount) { damageDealt += amount; }
    public void UpdateDamageTaken(float amount) { damageTaken += amount; }
    public void UpdateHealingDone(float amount) { healingDone += amount; }
}