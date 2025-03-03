using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Models;
using System.Collections.Generic;
public class DuelScript : MonoBehaviour
{
    public List<BaseUnit> playerUnits;
    public List<BaseUnit> enemyUnits;
    private List<BaseUnit> allUnits;

    public HealthBar healthBar;
    public SquadManager squadManager;
    private Dictionary<BaseUnit, HealthBar> unitHealthBars;

    public UIFight uiFight;

    private bool skipFight = false;
    private float fightSpeedMultiplier = 1f; // 1x, 2x, or 4x

    void Start()
    {
        squadManager = FindObjectOfType<SquadManager>();
        if (squadManager != null)
        {
            unitHealthBars = squadManager.unitHealthBars;
        }
        else
        {
            Debug.LogError("SquadManager not found in the scene.");
        }

        playerUnits.Clear();
        enemyUnits.Clear();
        Invoke("InitializeBattleDelayed", 1f);
    }

    public void UpdateHealth(BaseUnit unit, float newHealth)
    {
        if (unitHealthBars != null && unitHealthBars.ContainsKey(unit))
        {
            HealthBar healthBar = unitHealthBars[unit];
            healthBar.SetHealth((int)newHealth);
        }
        else
        {
            Debug.LogError("HealthBar not found for the given unit.");
        }
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
            aliveUnits.Sort((unit1, unit2) => unit2.baseSpeed.CompareTo(unit1.baseSpeed));

            foreach (BaseUnit unit in aliveUnits)
            {
                // If skip is true, skip the current unit's turn
                if (skipFight)
                {
                    continue;
                }

                // Ensure the unit is still alive before taking its turn
                if (!playerUnits.Contains(unit) && !enemyUnits.Contains(unit))
                    continue;

                Debug.Log($"Unit {unit.unitName} taking turn.");

                yield return StartCoroutine(ExecuteTurn(unit));

                yield return new WaitForSeconds(0.5f / fightSpeedMultiplier); // Adjust the delay based on speed multiplier
            }
        }

        EndBattle();
    }

    IEnumerator ExecuteTurn(BaseUnit unit)
    {
        // Ensure unit is still alive
        if (!playerUnits.Contains(unit) && !enemyUnits.Contains(unit))
            yield break;

        ApplyPassiveEffects(unit);

        if (unit.specialAttack.turnsToSpecial == 0)
            yield return StartCoroutine(ExecuteAttack(unit, unit.specialAttack));
        else
            yield return StartCoroutine(ExecuteAttack(unit, unit.normalAttack));

        Debug.Log($"Unit {unit.unitName} has completed its turn.");
    }

    void ApplyPassiveEffects(BaseUnit unit)
    {
        if (unit.passiveAbility != null)
        {
            Debug.Log($" {unit.unitName} is using passive {unit.passiveAbility.description}.");

            foreach (var effect in unit.passiveAbility.effects)
            {
                List<BaseUnit> targets = SelectTargets(unit, effect.targetType);

                foreach (var target in targets)
                {
                    Debug.Log($"Applying passive effect {effect.effectType} to {target.unitName}");
                    ApplyEffect(target, unit, effect);
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
        List<BaseUnit> enemies = new List<BaseUnit>();

        if (playerUnits.Contains(unit))
            enemies.AddRange(enemyUnits);
        else
            enemies.AddRange(playerUnits);

        return enemies;
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
                    ApplyEffect(target, attacker, effect);

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

        // Define front-row hex positions
        List<int> frontRow = new List<int> { 3, 6, 9 };

        // Determine if attacker is a player or enemy
        List<BaseUnit> allies = playerUnits.Contains(attacker) ? playerUnits : enemyUnits;
        List<BaseUnit> enemies = playerUnits.Contains(attacker) ? enemyUnits : playerUnits;

        if (targetType == BaseUnit.TargetType.Self)
        {
            targets.Add(attacker);
        }
        else if (targetType == BaseUnit.TargetType.SingleAlly)
        {
            targets.Add(GetRandomUnit(allies));
        }
        else if (targetType == BaseUnit.TargetType.AllAllies)
        {
            targets.AddRange(allies);
        }
        else if (targetType == BaseUnit.TargetType.SingleEnemy)
        {
            // Prioritize enemies in the front row
            List<BaseUnit> frontEnemies = enemies.FindAll(unit => frontRow.Contains(unit.HexId));

            if (frontEnemies.Count > 0)
            {
                targets.Add(GetRandomUnit(frontEnemies));
            }
            else
            {
                // If no front-row enemies exist, target any enemy
                targets.Add(GetRandomUnit(enemies));
            }
        }
        else if (targetType == BaseUnit.TargetType.AllEnemies)
        {
            // Prioritize front-row enemies first, then the rest
            List<BaseUnit> frontEnemies = enemies.FindAll(unit => frontRow.Contains(unit.HexId));
            if (frontEnemies.Count > 0)
            {
                targets.AddRange(frontEnemies);
            }
            else
            {
                targets.AddRange(enemies);
            }
        }

        return targets;
    }


    void ApplyEffect(BaseUnit target, BaseUnit attacker, BaseUnit.Effect effect)
    {
        Debug.Log($"[DEBUG] ApplyEffect to: {target.unitName} | Effect: {effect.effectType} | TargetedStat: {effect.targetedStat} | BaseValue: {effect.baseValue}");

        int calculatedValue = effect.baseValue;

        // Apply percentage scaling if applicable
        if (effect.isPercentage && string.IsNullOrEmpty(effect.scalingAttribute))
        {
            // If it's a percentage effect without a scaling attribute, apply percentage damage directly to the targeted stat (HP for example)
            if (effect.targetedStat.Equals("HP", System.StringComparison.OrdinalIgnoreCase))
            {
                // Apply percentage damage to HP (example: 5% of the target's HP)
                calculatedValue = (int)((target.maxHp * effect.baseValue) / 100f);
                Debug.Log($"[DEBUG] Calculated Percentage Damage: {calculatedValue} % {target.maxHp})");
            }
        }
        else
        {
            // Apply scaling with the attacker's stat (STR, for example)
            if (!string.IsNullOrEmpty(effect.scalingAttribute))
            {
                // Get the attacker's scaling stat value (e.g., STR, INT, etc.)
                int scalingStatValue = GetStat(attacker, effect.scalingAttribute); // Use the attacker's stat

                // Calculate scaling damage as a percentage of the attacker's stat (e.g., STR)
                int scaledDamage = (int)((effect.scalingPercent / 100.0f) * scalingStatValue);

                // Add the base value + scaled damage
                calculatedValue += scaledDamage;

                Debug.Log($"[DEBUG] Calculated Scaling Damage: {calculatedValue} (Scaling: {effect.scalingPercent}% of {scalingStatValue} from {effect.scalingAttribute})");
            }
        }

        // Apply effects based on type
        switch (effect.effectType)
        {
            case BaseUnit.EffectType.Damage:
                ApplyDamage(target, effect.targetedStat, calculatedValue); // Pass correct parameters
                break;
            case BaseUnit.EffectType.Heal:
                ApplyHealing(target, calculatedValue);
                break;
            case BaseUnit.EffectType.Buff:
                ModifyStat(target, effect.targetedStat, calculatedValue, false); // Buff = positive change
                break;
            case BaseUnit.EffectType.Debuff:
                ModifyStat(target, effect.targetedStat, calculatedValue, true); // Debuff = negative change
                break;
        }

        // Apply status effect if present
        if (effect.statusEffect != null)
        {
            ApplyStatusEffect(target, effect.statusEffect);
        }

        // Check if unit is defeated
        if (GetStat(target, "HP") <= 0)
        {
            Debug.Log($"{target.unitName} has been defeated!");
            StartCoroutine(RemoveUnitAfterDelay(target, 0.5f));
        }
    }
    void ApplyDamage(BaseUnit target, string defenseStat, int rawDamage)
    {
        // Handle HP damage and defense
        if (defenseStat.Equals("HP", System.StringComparison.OrdinalIgnoreCase))
        {
            target.baseHp -= rawDamage;
            target.baseHp = Mathf.Max(target.baseHp, 0); // Ensure HP doesn't go below 0
            Debug.Log($"{target.unitName} takes {rawDamage} damage. Remaining HP: {target.baseHp}");
            UpdateHealth(target, target.baseHp);
        }
        else
        {
            // Apply defense and calculate final damage
            int defenseValue = GetStat(target, defenseStat);
            int finalDamage = Mathf.Max(rawDamage - defenseValue, 1); // Ensure at least 1 damage

            int newHp = GetStat(target, "HP") - finalDamage;
            SetStat(target, "HP", newHp);
            UpdateHealth(target, newHp);

            Debug.Log($"{target.unitName} takes {finalDamage} damage. Remaining HP: {newHp}");
        }
    }





    void ApplyHealing(BaseUnit target, int healAmount)
    {
        int newHp = Mathf.Min(GetStat(target, "HP") + healAmount, target.maxHp);
        SetStat(target, "HP", newHp);
        UpdateHealth(target, newHp);

        Debug.Log($"{target.unitName} heals for {healAmount}. New HP: {newHp}");
    }

    void ModifyStat(BaseUnit target, string statName, int valueChange, bool isDebuff)
    {
        int currentValue = GetStat(target, statName);

        if (isDebuff)
        {
            valueChange = -Mathf.Abs(valueChange); // Ensure debuffs always subtract
        }

        SetStat(target, statName, currentValue + valueChange);

        Debug.Log($"{target.unitName}'s {statName} modified by {valueChange}. New value: {GetStat(target, statName)}");
    }



    void SetStat(BaseUnit unit, string statName, int newValue)
    {
        string statNameLower = statName.ToLower();
        Debug.Log($"Setting stat {statName} for {unit.unitName} to {newValue}");

        if (statNameLower == "strength" || statNameLower == "str")
            unit.baseStr = newValue;
        else if (statNameLower == "intelligence" || statNameLower == "int")
            unit.baseInt = newValue;
        else if (statNameLower == "speed" || statNameLower == "spd")
            unit.baseSpeed = newValue;
        else if (statNameLower == "pdef" || statNameLower == "physicaldefense")
            unit.pDef = newValue;
        else if (statNameLower == "mdef" || statNameLower == "magicaldefense")
            unit.mDef = newValue;
        else if (statNameLower == "hp" || statNameLower == "health")
            unit.baseHp = Mathf.Clamp(newValue, 0, unit.maxHp); // Ensure HP stays within range
        else
            Debug.LogError($"Stat '{statName}' not found for {unit.unitName}!");
    }



    IEnumerator RemoveUnitAfterDelay(BaseUnit unit, float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveUnit(unit);
    }

    int GetStat(BaseUnit unit, string statName)
    {
        string statNameLower = statName.ToLower();

        if (statNameLower == "strength" || statNameLower == "str")
            return unit.baseStr;
        if (statNameLower == "intelligence" || statNameLower == "int")
            return unit.baseInt;
        if (statNameLower == "speed" || statNameLower == "spd")
            return unit.baseSpeed;
        if (statNameLower == "pdef" || statNameLower == "physicaldefense")
            return unit.pDef;
        if (statNameLower == "mdef" || statNameLower == "magicaldefense")
            return unit.mDef;
        if (statNameLower == "hp" || statNameLower == "health")
            return unit.baseHp;

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
        SceneManager.LoadScene("MainMenuScene");
    }
}
