// StatusEffectManager.cs
using System.Collections;
using UnityEngine;

public static class StatusEffectManager
{
    /// <summary>
    /// The standard, animated method for processing start-of-turn effects. (Coroutine)
    /// </summary>
    public static IEnumerator ProcessStartOfTurnEffects(UnitFacade unit)
    {
        // --- POISON EFFECT ---
        if (unit.UnitData.isPoisoned)
        {
            int poisonDamage = Mathf.Max(1, Mathf.RoundToInt(unit.UnitData.maxHp * 0.1f));
            unit.UnitData.baseHp -= poisonDamage;
            unit.HealthBar.SetHealth(unit.UnitData.baseHp);
            EffectExecutor.ShowFloatingText(unit, poisonDamage.ToString(), Color.magenta);
            
            unit.UnitData.poisonDuration--;
            if (unit.UnitData.poisonDuration <= 0)
            {
                unit.UnitData.isPoisoned = false;
                EffectExecutor.ShowFloatingText(unit, "Poison Faded", Color.white);
            }
            if (unit.UnitData.baseHp <= 0) { BattleManager.Instance.HandleUnitDeath(unit); yield break; }
            yield return new WaitForSeconds(0.5f);
        }

        // --- DURATION DECREMENT ---
        if (unit.UnitData.isStunned)
        {
            unit.UnitData.stunDuration--;
            if (unit.UnitData.stunDuration <= 0) { unit.UnitData.isStunned = false; EffectExecutor.ShowFloatingText(unit, "Stun Cleared", Color.white); }
        }
        if (unit.UnitData.isTaunting)
        {
            unit.UnitData.tauntDuration--;
            if (unit.UnitData.tauntDuration <= 0) { unit.UnitData.isTaunting = false; EffectExecutor.ShowFloatingText(unit, "Taunt Faded", Color.white); }
        }
    }

    /// <summary>
    /// The instant, non-animated method for the "Skip" simulation. (Synchronous)
    /// </summary>
    public static void ProcessStartOfTurnEffectsSynchronous(UnitFacade unit)
    {
        // --- POISON EFFECT ---
        if (unit.UnitData.isPoisoned)
        {
            int poisonDamage = Mathf.Max(1, Mathf.RoundToInt(unit.UnitData.maxHp * 0.1f));
            unit.UnitData.baseHp -= poisonDamage;
            unit.HealthBar.SetHealth(unit.UnitData.baseHp); // Update data model
            // NOTE: The floating text call is safe because UnitAnimationController checks the IsFightSkipped flag.

            unit.UnitData.poisonDuration--;
            if (unit.UnitData.poisonDuration <= 0) unit.UnitData.isPoisoned = false;
            
            if (unit.UnitData.baseHp <= 0) { BattleManager.Instance.HandleUnitDeath(unit); return; } // Exit if dead
            // NO DELAY
        }

        // --- DURATION DECREMENT ---
        if (unit.UnitData.isStunned)
        {
            unit.UnitData.stunDuration--;
            if (unit.UnitData.stunDuration <= 0) unit.UnitData.isStunned = false;
        }
        if (unit.UnitData.isTaunting)
        {
            unit.UnitData.tauntDuration--;
            if (unit.UnitData.tauntDuration <= 0) unit.UnitData.isTaunting = false;
        }
    }
    
    // The ApplyStatusEffect method does not need a synchronous version as it is already instant.
    public static void ApplyStatusEffect(BaseUnit.StatusEffect statusData, UnitFacade user, UnitFacade target)
    {
        if (statusData.isRng && !DidAuraCheckSucceed(user.UnitData.aura)) return;
        switch (statusData.ccType)
        {
            case BaseUnit.CrowdControlType.Poison:
                target.UnitData.isPoisoned = true;
                target.UnitData.poisonDuration = statusData.duration;
                EffectExecutor.ShowFloatingText(target, "Poisoned!", Color.magenta);
                break;
            case BaseUnit.CrowdControlType.Stun:
                target.UnitData.isStunned = true;
                target.UnitData.stunDuration = statusData.duration;
                EffectExecutor.ShowFloatingText(target, "Stunned!", Color.yellow);
                break;
            case BaseUnit.CrowdControlType.Taunt:
                target.UnitData.isTaunting = true;
                target.UnitData.tauntDuration = statusData.duration;
                EffectExecutor.ShowFloatingText(target, "Taunting!", new Color(1.0f, 0.5f, 0.0f));
                break;
        }
    }

    private static bool DidAuraCheckSucceed(float auraPercentage) { return Random.Range(0f, 100f) <= auraPercentage; }
}