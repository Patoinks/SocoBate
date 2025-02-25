using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelScript : MonoBehaviour
{
    public List<BaseUnit> playerUnits;
    public List<BaseUnit> enemyUnits;
    private List<BaseUnit> allUnits;

    public HealthBar healthBar; 
    public SquadManager squadManager;
    public UIFight uiFight;

    void Start()
    {
        playerUnits.Clear();
        enemyUnits.Clear();
        Invoke("InitializeBattleDelayed", 1f);
    }

    void InitializeBattleDelayed()
    {
        allUnits = new List<BaseUnit>(playerUnits);
        allUnits.AddRange(enemyUnits);
        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        while (playerUnits.Count > 0 && enemyUnits.Count > 0)
        {
            List<BaseUnit> aliveUnits = new List<BaseUnit>(allUnits);

            // Sort the units by speed (higher speed first)
            aliveUnits.Sort((unit1, unit2) => unit2.baseSpeed.CompareTo(unit1.baseSpeed));

            foreach (BaseUnit unit in aliveUnits)
            {
                if (!playerUnits.Contains(unit) && !enemyUnits.Contains(unit))
                    continue;

                Debug.Log($"Unit {unit.unitName} taking turn.");

                yield return StartCoroutine(ExecuteTurn(unit));

                yield return new WaitForSeconds(0.5f);
            }
        }

        EndBattle();
    }


    IEnumerator ExecuteTurn(BaseUnit unit)
    {
        ApplyPassiveEffects(unit);

        if (unit.specialAttack.turnsToSpecial == 0)
            yield return StartCoroutine(ExecuteAttack(unit, unit.specialAttack));
        else
            yield return StartCoroutine(ExecuteAttack(unit, unit.normalAttack));

        Debug.Log($"Unit {unit.unitName} has completed its turn.");
    }

    void ApplyPassiveEffects(BaseUnit unit)
    {
        // Check if unit has passive abilities
        if (unit.passiveAbility != null)
        {
            Debug.Log($"Applying passive effects for {unit.unitName}.");

            foreach (var effect in unit.passiveAbility.effects)
            {
                Debug.Log($"Effect found: {effect.effectType} for {unit.unitName}");

                // Apply the debuff effect if it targets all enemies
                if (effect.effectType == BaseUnit.EffectType.Debuff)
                {
                    if (effect.targetType == BaseUnit.TargetType.AllEnemies)
                    {
                        Debug.Log($"Debuff effect targeting all enemies: {effect.targetedStat}");

                        // Get all enemy units based on the unit's allegiance
                        List<BaseUnit> targets = GetEnemyUnits(unit);

                        // Apply debuff to each enemy unit
                        foreach (var target in targets)
                        {
                            // Debugging output to track which units are being targeted
                            Debug.Log($"Applying debuff {effect.effectType} to {target.unitName}");

                            if (target != unit)  // Exclude the caster
                            {
                                ApplyEffect(target, effect);  // Apply the debuff
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"{unit.unitName} does not have any passive abilities.");
        }
    }


    List<BaseUnit> GetEnemyUnits(BaseUnit unit)
    {
        List<BaseUnit> enemyUnits = new List<BaseUnit>();
        if (playerUnits.Contains(unit))
        {
            // If the unit is a player unit, target all enemy units
            enemyUnits.AddRange(enemyUnits);
        }
        else
        {
            // If the unit is an enemy, target all player units
            enemyUnits.AddRange(playerUnits);
        }

        return enemyUnits;
    }



    IEnumerator ExecuteAttack(BaseUnit attacker, BaseUnit.AttackData attackData)
    {
        Debug.Log($"{attacker.unitName} is attacking with: {attackData.description}");

        foreach (var effect in attackData.effects)
        {
            List<BaseUnit> targets = SelectTargets(attacker, effect.targetType);

            foreach (BaseUnit target in targets)
            {
                if (target != null)
                {
                    Debug.Log($"Applying effect {effect.effectType} to {target.unitName}");
                    ApplyEffect(target, effect);

                    if (effect.effectType == BaseUnit.EffectType.Damage)
                    {
                        if (uiFight != null)
                        {
                            yield return StartCoroutine(uiFight.MoveUnitCloser(target, attacker));
                        }
                        else
                        {
                            Debug.LogError("UIFight is not assigned!");
                        }
                    }
                }
            }
        }
    }

    List<BaseUnit> SelectTargets(BaseUnit attacker, BaseUnit.TargetType targetType)
    {
        List<BaseUnit> targets = new List<BaseUnit>();

        if (targetType == BaseUnit.TargetType.Self)
        {
            targets.Add(attacker);
        }
        else if (targetType == BaseUnit.TargetType.SingleAlly)
        {
            if (playerUnits.Contains(attacker))
                targets.Add(GetRandomUnit(playerUnits));
            else
                targets.Add(GetRandomUnit(enemyUnits));
        }
        else if (targetType == BaseUnit.TargetType.AllAllies)
        {
            if (playerUnits.Contains(attacker))
                targets.AddRange(playerUnits);
            else
                targets.AddRange(enemyUnits);
        }
        else if (targetType == BaseUnit.TargetType.SingleEnemy)
        {
            if (playerUnits.Contains(attacker))
                targets.Add(GetRandomUnit(enemyUnits));  // Player attacks enemy
            else
                targets.Add(GetRandomUnit(playerUnits)); // Enemy attacks player
        }
        else if (targetType == BaseUnit.TargetType.AllEnemies)
        {
            if (playerUnits.Contains(attacker))
                targets.AddRange(enemyUnits);
            else
                targets.AddRange(playerUnits);
        }

        // Ensure that the caster is not included as the target for debuffs
        if (attacker != null)
        {
            targets.Remove(attacker); // Remove the attacker from the list of targets if it's included
        }

        return targets;
    }



    void ApplyEffect(BaseUnit target, BaseUnit.Effect effect)
    {
        Debug.Log($"{target.unitName} is affected by {effect.effectType} with {effect.baseValue} {effect.targetedStat}.");

        int calculatedValue = effect.baseValue;

        // Check if the effect is a percentage-based calculation
        if (effect.isPercentage)
        {
            // Apply scaling percentage for intelligence
            if (effect.scalingAttribute.Equals("Intelligence", System.StringComparison.OrdinalIgnoreCase))
            {
                int scalingStatValue = target.baseInt; // Get the intelligence value of the target
                calculatedValue += (int)(scalingStatValue * (effect.scalingPercent / 100.0f)); // 20% of intelligence + base value
            }
            else
            {
                // If not Intelligence, use the existing logic for other stats (like Strength)
                int scalingStatValue = GetStat(target, effect.scalingAttribute);
                calculatedValue += (int)((effect.scalingPercent / 100.0f) * scalingStatValue);
            }
        }
        else
        {
            // If not percentage-based, use the existing baseValue calculation logic
            int scalingStatValue = GetStat(target, effect.scalingAttribute);
            calculatedValue += (int)((effect.scalingPercent / 100.0f) * scalingStatValue);
        }

        // Apply the damage calculation
        if (effect.effectType == BaseUnit.EffectType.Damage)
        {
            int finalDamage = calculatedValue;
            if (effect.targetedStat.Equals("PDEF", System.StringComparison.OrdinalIgnoreCase))
            {
                finalDamage -= target.pDef;
            }
            else if (effect.targetedStat.Equals("MDEF", System.StringComparison.OrdinalIgnoreCase))
            {
                finalDamage -= target.mDef;
            }

            finalDamage = Mathf.Max(finalDamage, 0); // Ensure damage cannot go below 0
            target.baseHp -= finalDamage; // Apply the damage to the target's HP
            healthBar.UpdateHealth(target.baseHp);  
            Debug.Log($"{target.unitName} takes {finalDamage} damage. Remaining HP: {target.baseHp}");
        }
        else if (effect.effectType == BaseUnit.EffectType.Heal)
        {
            target.baseHp += calculatedValue;
            Debug.Log($"{target.unitName} heals {calculatedValue}. New HP: {target.baseHp}");
        }
        else if (effect.effectType == BaseUnit.EffectType.Buff)
        {
            // Apply Buff effects based on targeted stat (defense, speed, etc.)
            if (effect.targetedStat.Equals("PDEF", System.StringComparison.OrdinalIgnoreCase))
            {
                target.pDef += calculatedValue;
            }
            else if (effect.targetedStat.Equals("MDEF", System.StringComparison.OrdinalIgnoreCase))
            {
                target.mDef += calculatedValue;
            }
            else if (effect.targetedStat.Equals("Speed", System.StringComparison.OrdinalIgnoreCase))
            {
                target.baseSpeed += calculatedValue;
            }
        }
        else if (effect.effectType == BaseUnit.EffectType.Debuff)
        {
            // Apply Debuff effects based on targeted stat (defense, speed, etc.)
            if (effect.targetedStat.Equals("PDEF", System.StringComparison.OrdinalIgnoreCase))
            {
                target.pDef -= calculatedValue;
            }
            else if (effect.targetedStat.Equals("MDEF", System.StringComparison.OrdinalIgnoreCase))
            {
                target.mDef -= calculatedValue;
            }
            else if (effect.targetedStat.Equals("Speed", System.StringComparison.OrdinalIgnoreCase))
            {
                target.baseSpeed -= calculatedValue;
            }
        }

        if (effect.statusEffect != null)
        {
            ApplyStatusEffect(target, effect.statusEffect);
        }

        if (target.baseHp <= 0)
        {
            Debug.Log($"{target.unitName} has been defeated!");
            RemoveUnit(target);
        }
    }


    int GetStat(BaseUnit unit, string statName)
    {
        string statNameLower = statName.ToLower();
        if (statNameLower == "strength" || statNameLower == "str")
            return unit.baseStr;
        if (statNameLower == "int" || statNameLower == "intelligence")
            return unit.baseInt;
        return 0;
    }

    void ApplyStatusEffect(BaseUnit target, BaseUnit.StatusEffect statusEffect)
    {
        if (statusEffect.ccType == BaseUnit.CrowdControlType.Poison)
        {
            int poisonDamage = (int)(statusEffect.tickDamage + (statusEffect.scalingPercent * target.baseInt / 100));
            target.baseHp -= poisonDamage;
            Debug.Log($"{target.unitName} takes {poisonDamage} poison damage. HP left: {target.baseHp}");
        }
    }

    void RemoveUnit(BaseUnit unit)
    {
        if (playerUnits.Contains(unit))
        {
            playerUnits.Remove(unit);
        }
        else if (enemyUnits.Contains(unit))
        {
            enemyUnits.Remove(unit);
        }

        allUnits.Remove(unit);
    }

    BaseUnit GetRandomUnit(List<BaseUnit> units)
    {
        if (units.Count == 0) return null;
        int randomIndex = Random.Range(0, units.Count);
        return units[randomIndex];
    }

    void EndBattle()
    {
        Debug.Log("The battle is over!");
    }
}
