using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelScript : MonoBehaviour
{
    public List<BaseUnit> playerUnits; // Player's team
    public List<BaseUnit> enemyUnits; // Enemy's team
    private List<BaseUnit> allUnits; // Combined list for turn order

    void Start()
    {
        InitializeBattle();
    }

    void InitializeBattle()
    {
        // Load units into battle from serialized data
        allUnits = new List<BaseUnit>(playerUnits);
        allUnits.AddRange(enemyUnits);
        
        // Sort by speed for turn order
        allUnits.Sort((a, b) => b.baseSpeed.CompareTo(a.baseSpeed));

        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        while (playerUnits.Count > 0 && enemyUnits.Count > 0)
        {
            foreach (BaseUnit unit in allUnits)
            {
                if (!playerUnits.Contains(unit) && !enemyUnits.Contains(unit))
                    continue; // Skip dead units

                ExecuteTurn(unit);
                yield return new WaitForSeconds(1f); // Delay for turn simulation
            }
        }

        EndBattle();
    }

    void ExecuteTurn(BaseUnit unit)
    {
        ApplyPassiveEffects(unit);
        
        if (unit.specialAttack.turnsToSpecial == 0)
            ExecuteAttack(unit, unit.specialAttack);
        else
            ExecuteAttack(unit, unit.normalAttack);
    }

    void ApplyPassiveEffects(BaseUnit unit)
    {
        foreach (var effect in unit.passiveAbility.effects)
        {
            ApplyEffect(unit, effect);
        }
    }

    void ExecuteAttack(BaseUnit attacker, BaseUnit.AttackData attackData)
    {
        foreach (var effect in attackData.effects)
        {
            BaseUnit target = SelectTarget(attacker, effect.targetType);
            if (target != null)
            {
                ApplyEffect(target, effect);
            }
        }
    }

    BaseUnit SelectTarget(BaseUnit attacker, BaseUnit.TargetType targetType)
    {
        if (targetType == BaseUnit.TargetType.Self) return attacker;
        if (targetType == BaseUnit.TargetType.SingleAlly) return GetRandomUnit(playerUnits);
        if (targetType == BaseUnit.TargetType.AllAllies) return GetRandomUnit(playerUnits); // Modify for AOE logic
        if (targetType == BaseUnit.TargetType.SingleEnemy) return GetRandomUnit(enemyUnits);
        if (targetType == BaseUnit.TargetType.AllEnemies) return GetRandomUnit(enemyUnits); // Modify for AOE logic
        return null;
    }

    void ApplyEffect(BaseUnit target, BaseUnit.Effect effect)
    {
        int calculatedValue = (effect.baseValue + (effect.scalingPercent * GetStat(target, effect.scalingAttribute) / 100));
        if (effect.effectType == BaseUnit.EffectType.Damage)
            target.baseHp -= calculatedValue;
        else if (effect.effectType == BaseUnit.EffectType.Heal)
            target.baseHp += calculatedValue;

        if (target.baseHp <= 0)
            RemoveUnit(target);
    }

    int GetStat(BaseUnit unit, string statName)
    {
        if (statName == "Strength") return unit.baseStr;
        if (statName == "Intelligence") return unit.baseInt;
        return 0;
    }

    void RemoveUnit(BaseUnit unit)
    {
        playerUnits.Remove(unit);
        enemyUnits.Remove(unit);
        allUnits.Remove(unit);
    }

    BaseUnit GetRandomUnit(List<BaseUnit> unitList)
    {
        if (unitList.Count == 0) return null;
        return unitList[Random.Range(0, unitList.Count)];
    }

    void EndBattle()
    {
        if (playerUnits.Count == 0)
            Debug.Log("Enemy Wins!");
        else
            Debug.Log("Player Wins!");
    }
}
