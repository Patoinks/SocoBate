// EffectExecutor.cs
using UnityEngine;
using TMPro;

public static class EffectExecutor
{
    public static void ShowFloatingText(UnitFacade target, string text, Color color)
    {
        if (target == null) return;
        UnitAnimationController.Instance.RequestFloatingText(text, color, target);
    }
    
    public static void ApplyEffect(BaseUnit.Effect effect, UnitFacade user, UnitFacade target)
    {
        if (target == null) return;
        int finalValue = CalculateScaledValue(effect, user.UnitData);
        switch (effect.effectType)
        {
            case BaseUnit.EffectType.Damage: ApplyDamage(finalValue, effect.targetedStat, user, target); break;
            case BaseUnit.EffectType.Heal: ApplyHealing(finalValue, user, target); break;
            case BaseUnit.EffectType.Buff: ModifyStat(effect.targetedStat, finalValue, target, false); break;
            case BaseUnit.EffectType.Debuff: ModifyStat(effect.targetedStat, finalValue, target, true); break;
            case BaseUnit.EffectType.Steal: ApplySteal(effect.targetedStat, effect.baseValue / 100f, user, target); break;
            case BaseUnit.EffectType.Odds: ApplyOdds(effect, user, target); break;
        }
        if (effect.statusEffect != null && effect.statusEffect.ccType != BaseUnit.CrowdControlType.None)
        {
            StatusEffectManager.ApplyStatusEffect(effect.statusEffect, user, target);
        }
        if (target.UnitData.baseHp <= 0)
        {
            BattleManager.Instance.HandleUnitDeath(target);
        }
    }

    private static void ApplyDamage(int rawDamage, string defenseStat, UnitFacade user, UnitFacade target)
    {
        Color damageColor = Color.red;
        rawDamage = Mathf.Max(0, rawDamage);
        int finalDamage;
        string defenseLogMessage;
        if (string.Equals(defenseStat, "TRUE", System.StringComparison.OrdinalIgnoreCase))
        {
            finalDamage = rawDamage;
            defenseLogMessage = "TRUE (Bypassed)";
        }
        else
        {
            int defenseValue = GetStat(target.UnitData, defenseStat);
            float damageMultiplier = 100f / (100f + defenseValue);
            finalDamage = Mathf.RoundToInt(rawDamage * damageMultiplier);
            finalDamage = Mathf.Max(1, finalDamage);
            defenseLogMessage = defenseValue.ToString();
        }
        target.UnitData.baseHp -= finalDamage;
        user.UnitData.UpdateDamageDealt(finalDamage);
        target.UnitData.UpdateDamageTaken(finalDamage);
        target.HealthBar.SetHealth(target.UnitData.baseHp);
        ShowFloatingText(target, $"-{finalDamage} HP", damageColor);
        Debug.Log($"{user.UniqueID} dealt {finalDamage} damage to {target.UniqueID} (Raw: {rawDamage}, Defense Value: {defenseLogMessage})");
    }

    private static void ApplyHealing(int amount, UnitFacade user, UnitFacade target)
    {
        Color healColor = Color.green;
        int currentHp = target.UnitData.baseHp;
        int maxHp = target.UnitData.maxHp;
        int finalHeal = Mathf.Min(amount, maxHp - currentHp);
        target.UnitData.baseHp += finalHeal;
        user.UnitData.UpdateHealingDone(finalHeal);
        target.HealthBar.SetHealth(target.UnitData.baseHp);
        ShowFloatingText(target, $"+{finalHeal} HP", healColor);
    }

    // --- THIS IS THE MODIFIED METHOD ---
    private static void ModifyStat(string stat, int amount, UnitFacade target, bool isDebuff)
    {
        Color buffColor = Color.cyan;
        Color debuffColor = new Color(1.0f, 0.5f, 0.0f); // Orange
        
        int finalAmount = isDebuff ? -amount : amount;
        SetStat(target.UnitData, stat, GetStat(target.UnitData, stat) + finalAmount);
        
        // --- THIS IS THE FIX ---
        // We now explicitly check if the amount is positive or negative to build the string.
        string prefix;
        Color finalColor;

        if (isDebuff)
        {
            prefix = "-";
            finalColor = debuffColor;
        }
        else // It's a buff
        {
            prefix = "+";
            finalColor = buffColor;
        }
        
        ShowFloatingText(target, $"{prefix}{amount} {stat.ToUpper()}", finalColor);
    }
    
    private static void ApplyOdds(BaseUnit.Effect effect, UnitFacade user, UnitFacade target)
    {
        float roll = Random.Range(0f, 100f);
        float chanceToSucceed = effect.scalingPercent;
        if (roll <= chanceToSucceed)
        {
            ModifyStat(effect.targetedStat, effect.baseValue, target, false);
        }
        else
        {
            ModifyStat(effect.targetedStat, effect.baseValue, target, true);
        }
    }

    #region Unchanged Methods
    private static int CalculateScaledValue(BaseUnit.Effect effect, BaseUnit user)
    {
        if (!string.IsNullOrEmpty(effect.scalingAttribute) && effect.scalingPercent > 0 && effect.effectType != BaseUnit.EffectType.Odds)
        {
            int scalingStatValue = GetStat(user, effect.scalingAttribute);
            float scaledAmount = scalingStatValue * (effect.scalingPercent / 100f);
            return effect.baseValue + Mathf.RoundToInt(scaledAmount);
        }
        return effect.baseValue;
    }
    
    private static void ApplySteal(string stat, float percentage, UnitFacade user, UnitFacade target)
    {
        int stolenAmount = Mathf.RoundToInt(GetStat(target.UnitData, stat) * percentage);
        ModifyStat(stat, stolenAmount, user, false);
        ModifyStat(stat, stolenAmount, target, true);
    }
    
    private static int GetStat(BaseUnit unit, string statName)
    {
        string statNameLower = statName.ToLower();
        switch (statNameLower)
        {
            case "strength": case "str": return unit.baseStr;
            case "intelligence": case "int": return unit.baseInt;
            case "speed": case "spd": return unit.baseSpeed;
            case "pdef": case "physicaldefense": return unit.pDef;
            case "mdef": case "magicaldefense": return unit.mDef;
            case "hp": case "health": return unit.baseHp;
            case "aura": return unit.aura;
            default: Debug.LogError($"Stat '{statName}' not found!"); return 0;
        }
    }

    private static void SetStat(BaseUnit unit, string statName, int newValue)
    {
        string statNameLower = statName.ToLower();
        switch (statNameLower)
        {
            case "strength": case "str": unit.baseStr = Mathf.Max(0, newValue); break;
            case "intelligence": case "int": unit.baseInt = Mathf.Max(0, newValue); break;
            case "speed": case "spd": unit.baseSpeed = Mathf.Max(0, newValue); break;
            case "pdef": case "physicaldefense": unit.pDef = Mathf.Max(0, newValue); break;
            case "mdef": case "magicaldefense": unit.mDef = Mathf.Max(0, newValue); break;
            case "aura": unit.aura = Mathf.Max(0, newValue); break;
            case "hp": case "health": unit.baseHp = Mathf.Clamp(newValue, 0, unit.maxHp); break;
            default: Debug.LogError($"Stat '{statName}' not found!"); break;
        }
    }
    #endregion
}