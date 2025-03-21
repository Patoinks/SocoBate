using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Models;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class DuelScript : MonoBehaviour
{
    public GameObject damageEffectPrefab;
    public List<BaseUnit> playerUnits;
    public List<BaseUnit> enemyUnits;
    private List<BaseUnit> allUnits;
    public GameObject menu;
    public HealthBar healthBar;
    public SquadManager squadManager;
    private Dictionary<BaseUnit, HealthBar> unitHealthBars;
    public int turnCounter = 0;
    public TMP_Text TurnCounterText;
    public List<BaseUnit> deadPlayerUnits = new List<BaseUnit>();
    public List<BaseUnit> deadEnemyUnits = new List<BaseUnit>();

    public UIFight uiFight;
    private bool skipFight = false;

    public float duration = 0.3f;

    public float turnSpeed = 1f;

    public int winner = 0;

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
        SetUIInteractivity(false);
        StartCoroutine(InitializeBattleWithFetch());
    }

    IEnumerator InitializeBattleWithFetch()
    {
        yield return new WaitForSeconds(3f); // Give time for units to initialize

        // Ensure all units are actually present
        if (playerUnits.Count == 0 || enemyUnits.Count == 0)
        {
            Debug.LogError("Units are not properly initialized before fetching!");
        }
        else
        {
            Debug.Log("All units are ready. Fetching...");
        }

        InitializeBattleDelayed(); // Proceed with battle setup
        SetUIInteractivity(true);
    }

    void SetUIInteractivity(bool isInteractable)
    {
        Button[] buttons = FindObjectsOfType<Button>(); // Find all buttons in the scene
        foreach (Button button in buttons)
        {
            button.interactable = isInteractable; // Enable or disable buttons
        }

        // If you have other interactable elements like sliders, you can disable them too
        // Example for sliders:
        Slider[] sliders = FindObjectsOfType<Slider>();
        foreach (Slider slider in sliders)
        {
            slider.interactable = isInteractable;
        }

        // You can also disable other UI elements like panels or images if necessary
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

    public int TurnCounter()
    {
        turnCounter++;
        TurnCounterText.text = $"Turn: {turnCounter}";
        return turnCounter;
    }

    void EnableOutline(GameObject unitPrefab, bool enable)
    {
        Outline outline = unitPrefab.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = enable;
        }
    }

    IEnumerator BattleLoop()
    {
        while (playerUnits.Count > 0 && enemyUnits.Count > 0)
        {

            TurnCounter();
            // Ensure that the list of all alive units is updated after each turn
            List<BaseUnit> aliveUnits = new List<BaseUnit>(playerUnits);
            aliveUnits.AddRange(enemyUnits);

            // Sort units by speed or another priority factor
            // Sort units by speed, breaking ties randomly
            aliveUnits.Sort((unit1, unit2) =>
            {
                int speedComparison = unit2.baseSpeed.CompareTo(unit1.baseSpeed); // Higher speed first
                if (speedComparison == 0)
                {
                    return Random.Range(0, 2) == 0 ? -1 : 1; // Randomly decide order if speed is equal
                }
                return speedComparison;
            });


            // If there are no alive units, break the loop early
            if (playerUnits.Count == 0)
            {
                break;
            }
            else if (enemyUnits.Count == 0)
            {
                break;
            }

            foreach (BaseUnit unit in aliveUnits)
            {
                GameObject unitPrefab = uiFight.FindUnitInHexContainer(unit);
                if (unitPrefab != null)
                {
                    EnableOutline(unitPrefab, true); // Turn on outline
                }


                // Skip units that are dead
                if (!playerUnits.Contains(unit) && !enemyUnits.Contains(unit))
                    continue;

                CheckStatusEffects(unit);
                if (unit.isStunned)
                {
                    Debug.Log($"{unit.unitName} is stunned and cannot take its turn.");
                    continue;
                }

                if (unit.DapAttemptChance > 0)
                {
                    yield return StartCoroutine(DapAttempt(unit));
                }

                Debug.Log($"Unit {unit.unitName} taking turn.");
                yield return StartCoroutine(ExecuteTurn(unit));
                if (unitPrefab != null)
                {
                    EnableOutline(unitPrefab, false); // Turn off outline
                }
            }
        }


        if (playerUnits.Count == 0)
        {
            winner = 1;
        }
        else if (enemyUnits.Count == 0)
        {
            winner = 2;
        }

        EndBattle();
    }


    public void SkipFight()
    {
        skipFight = true;
    }

    IEnumerator ExecuteTurn(BaseUnit unit)
    {
        // Ensure unit is still alive
        if (playerUnits.Count == 0 || enemyUnits.Count == 0)
        {
            yield break; // Exit early if no units are left
        }

        // Example in ExecuteTurn method (line 130 in your stack trace)
        Debug.Log($"[DEBUG] Applying passive effects to unit: {unit?.unitName ?? "null unit"}");
        yield return StartCoroutine(ApplyPassiveEffectsWithDelay(unit)); // Wait for passive effects to complete

        if (unit.specialAttack.turnsToSpecial == 0)
            yield return StartCoroutine(ExecuteAttack(unit, unit.specialAttack));
        else
            yield return StartCoroutine(ExecuteAttack(unit, unit.normalAttack));

        Debug.Log($"Unit {unit.unitName} has completed its turn.");
    }

    IEnumerator ApplyPassiveEffectsWithDelay(BaseUnit unit)
    {
        // Apply passive effects
        yield return StartCoroutine(ApplyPassiveEffects(unit));  // Wait for passive effects to complete

        if (skipFight == true)
        {
            yield return new WaitForSeconds(0);  // Delay if not skipping
        }
        else
        {
            yield return new WaitForSeconds(turnSpeed);  // Delay if not skipping
        }

    }
    IEnumerator ApplyPassiveEffects(BaseUnit unit)
    {
        if (unit.passiveAbility != null)
        {
            Debug.Log($"[DEBUG] {unit.unitName} is using passive ability: {unit.passiveAbility.description}");

            foreach (var effect in unit.passiveAbility.effects)
            {
                // Log the effect details
                Debug.Log($"[DEBUG] Processing effect: {effect.effectType} targeting: {effect.targetedStat} with base value: {effect.baseValue}");

                List<BaseUnit> targets = SelectTargets(unit, effect.targetType);

                if (targets == null || targets.Count == 0)
                {
                    Debug.LogWarning($"[WARNING] No valid targets found for passive effect on {unit.unitName} with type {effect.targetType}");
                }
                else
                {
                    foreach (var target in targets)
                    {
                        // Check if target is still alive
                        if (GetStat(target, "HP") <= 0)
                        {
                            Debug.LogWarning($"[WARNING] {target.unitName} is dead and cannot be affected by passive effect.");
                            continue; // Skip dead targets
                        }

                        // Log target before applying effect
                        Debug.Log($"[DEBUG] Applying passive effect {effect.effectType} to {target.unitName} with base value {effect.baseValue}");
                        ApplyEffect(target, unit, effect);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"[WARNING] {unit.unitName} does not have any passive abilities.");
        }

        yield break; // Ensure it returns an IEnumerator as required
    }



    public IEnumerator DapAttempt(BaseUnit unit)
    {
        Debug.Log("alo");
        // Determine if the unit is a player or enemy to get the correct team
        List<BaseUnit> team = playerUnits.Contains(unit) ? playerUnits : enemyUnits;

        // Iterate through all teammates (excluding itself)
        foreach (BaseUnit targetUnit in team)
        {
            Debug.Log($"{unit.unitName} is attempting to dap with {targetUnit.unitName}.");
            if (unit == targetUnit)
            {
                // Skip dap attempt with itself
                continue;
            }

            // First, check if the dap attempt is even possible based on the unit's attempt chance
            float attemptRoll = Random.Range(0f, 100f); // Random value between 0 and 100

            if (attemptRoll <= unit.DapAttemptChance)
            {
                // Dap attempt is successful, now check if the dap is successful based on the dap success chance
                float successRoll = Random.Range(0f, 100f); // Random value between 0 and 100

                if (successRoll <= unit.DapSuccessChance)
                {
                    // Both units move towards the center of the field with offsets
                    Debug.Log($"{unit.unitName} successfully dapped {targetUnit.unitName}!");

                    if (skipFight == true)
                    {
                    }
                    else
                    {
                        if (playerUnits.Contains(unit))
                        {
                            yield return StartCoroutine(MoveUnitsToDapPosition(targetUnit, unit, duration, false));

                        }
                        else
                        {
                            yield return StartCoroutine(MoveUnitsToDapPosition(unit, targetUnit, duration, true));

                        }
                    }

                    // After reaching the center, the dap occurs
                    Debug.Log($"{unit.unitName} and {targetUnit.unitName} are now dapping!");

                    ModifyStat(unit, "Aura", 10, false); // Increase unit's aura by 10
                    ModifyStat(targetUnit, "Aura", 10, false); // Increase target's aura by 10
                }
                else
                {
                    // Dap failed
                    Debug.Log($"{unit.unitName} failed to dap {targetUnit.unitName}!");
                    // Apply any effects for failure if necessary
                }
            }
            else
            {
                // Dap attempt failed because it didn't meet the attempt chance
                Debug.Log($"{unit.unitName} failed to attempt a dap with {targetUnit.unitName}.");
            }
        }
    }

    private IEnumerator MoveUnitsToDapPosition(BaseUnit unit, BaseUnit targetUnit, float duration, bool Rotater)
    {
        Coroutine moveUnit1 = StartCoroutine(uiFight.MoveUnitToDapPosition(unit, false, duration, Rotater));
        Coroutine moveUnit2 = StartCoroutine(uiFight.MoveUnitToDapPosition(targetUnit, true, duration, Rotater));

        // Wait for both to finish
        yield return moveUnit1;
        yield return moveUnit2;
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
                            if (!skipFight)
                            {
                                yield return StartCoroutine(uiFight.MoveUnitCloser(target, attacker, duration));
                            }
                            else
                            {
                                Debug.Log("Skipping fight animation.");
                            }

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

        // Check for taunting enemies
        List<BaseUnit> tauntingEnemies = enemies.FindAll(unit => unit.isTaunting); // Assuming 'isTaunting' is a property of BaseUnit


        // No taunting enemy, proceed with normal target selection
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
        // If there are any taunting enemies, prioritize them
        else if (tauntingEnemies.Count > 0)
        {
            targets.AddRange(tauntingEnemies); // Focus only on taunting enemies
        }
        else
        {
            if (targetType == BaseUnit.TargetType.SingleEnemy)
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
        }

        Debug.Log($"Selected {targets.Count} targets for {attacker.unitName} with target type: {targetType}");
        return targets;
    }

    public void OnClickFightSpeed()
    {
        Debug.Log("Changing fight speed.");
        if (duration == 0.3f)
        {
            turnSpeed = 0.5f;
            duration = 0.15f;
        }
        else
        {
            turnSpeed = 1f;
            duration = 0.3f;
        }
    }

    public void ApplyStealingStats(BaseUnit sourceUnit, BaseUnit targetUnit, float stealPercentage, string targetedStat)
    {
        // Log for debugging purposes
        Debug.Log($"Stealing {targetedStat} from {targetUnit.unitName} and applying to {sourceUnit.unitName}.");

        // Get the target unit's stat value
        int targetStatValue = GetStat(targetUnit, targetedStat);

        // Calculate the stolen amount based on the specified percentage
        int stolenAmount = Mathf.RoundToInt(targetStatValue * stealPercentage);

        // Apply the stolen amount to the source unit
        int newSourceStatValue = GetStat(sourceUnit, targetedStat) + stolenAmount;
        SetStat(sourceUnit, targetedStat, newSourceStatValue); // FIXED

        // Reduce the target's stat
        int newTargetStatValue = Mathf.Max(0, targetStatValue - stolenAmount);
        SetStat(targetUnit, targetedStat, newTargetStatValue);

        // Log the result
        Debug.Log($"Stole {stolenAmount} {targetedStat} from {targetUnit.unitName}. New {sourceUnit.unitName} {targetedStat}: {newSourceStatValue}, Target {targetUnit.unitName} {targetedStat}: {newTargetStatValue}");
    }

    public void ShowEffectOnUnit(BaseUnit unit, string effectText, Color effectColor, string stat, bool isBufforDebuff)
    {
        GameObject unitPrefab = uiFight.FindUnitInHexContainer(unit); // Find the unit's prefab in the scene.
        if (unitPrefab != null)
        {
            // Find the Canvas in the scene
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                string unitHexName = unitPrefab.transform.parent != null ? unitPrefab.transform.parent.name : "";

                // Create the effect after finding the unit
                Vector3 effectPosition = unitPrefab.transform.position; // Slightly above the unit's hex.

                // Instantiate the effect under the canvas and set it up with necessary data
                GameObject effectInstance = Instantiate(damageEffectPrefab);
                effectInstance.transform.SetParent(canvas.transform, false); // Set parent to canvas

                // Set the world position to the desired position (above the unit)
                effectInstance.transform.position = effectPosition;

                // Assuming the prefab has a TextMeshPro component
                TMP_Text textComponent = effectInstance.GetComponent<TMP_Text>();
                if (textComponent != null)
                {
                    if (isBufforDebuff == false)
                    {
                        textComponent.text = effectText;  // Set the effect text (like damage or heal).
                        textComponent.color = effectColor; // Set the text color (red, green, etc.).
                    }
                    else
                    {
                        textComponent.text = effectText + " " + stat;  // Set the effect text (like damage or heal).
                        textComponent.color = effectColor; // Set the text color (red, green, etc.).
                    }
                }

                // Destroy the effect after 1.5 seconds
                Destroy(effectInstance, turnSpeed);
            }
            else
            {
                Debug.LogError("Canvas not found in the scene.");
            }
        }
        else
        {
            Debug.LogError("Unit prefab not found.");
        }
    }


    void ApplyOdds(BaseUnit attacker, BaseUnit target, int baseValue, string targetedStat, int scalingPercent)
    {
        // Log for debugging purposes
        Debug.Log($"Applying odds effect ({scalingPercent}%) to {target.unitName}.");

        // Calculate the odds of the effect
        float odds = Random.Range(0f, 100f); // Random value between 0 and 100

        // Check if the odds are within the specified range (success)
        if (odds <= scalingPercent)
        {
            // Log the successful application
            Debug.Log($"Odds effect succeeded for {target.unitName}. Applied {baseValue} {targetedStat}.");
            ModifyStat(target, targetedStat, baseValue, false); // Apply positive effect (buff)
        }
        else
        {
            ModifyStat(target, targetedStat, baseValue, true); // Apply negative effect (debuff)
            Debug.Log($"Odds effect failed for {target.unitName}.");
        }
    }


    void ModifyStat(BaseUnit target, string statName, int valueChange, bool isDebuff)
    {
        int currentValue = GetStat(target, statName);

        if (isDebuff)
        {
            valueChange = -Mathf.Abs(valueChange); // Ensure debuffs always subtract
        }

        SetStat(target, statName, currentValue + valueChange);

        // Rotate unit every time a stat is modified

        if (!skipFight)
            StartCoroutine(uiFight.RotateUnit(target, duration)); // Rotate for 1 second (you can adjust this duration)



        if (!skipFight)
            ShowEffectOnUnit(target, valueChange.ToString(), isDebuff ? Color.black : Color.blue, statName, true);

        Debug.Log($"{target.unitName}'s {statName} modified by {valueChange}. New value: {GetStat(target, statName)}");
    }


    void ApplyEffect(BaseUnit target, BaseUnit attacker, BaseUnit.Effect effect)
    {

        int baseValueCalculated = 0;

        if (effect.effectType == BaseUnit.EffectType.Steal)
        {
            ApplyStealingStats(attacker, target, effect.baseValue, effect.targetedStat); // Steal = steal percentage of target's stats
        }


        if (effect.effectType == BaseUnit.EffectType.Odds)
        {
            ApplyOdds(attacker, target, effect.baseValue, effect.targetedStat, effect.scalingPercent); // Steal = steal percentage of target's stats
        }


        Debug.Log($"[DEBUG] ApplyEffect to: {target.unitName} | Effect: {effect.effectType} | TargetedStat: {effect.targetedStat} | BaseValue: {effect.baseValue}");

        if (effect.scalingAttribute != null && effect.scalingPercent > 0)
        {
            // Calculate the scaled base value based on attacker's stat
            baseValueCalculated = effect.baseValue + Mathf.FloorToInt(GetStat(attacker, effect.scalingAttribute) * (effect.scalingPercent / 100f));

            Debug.Log($"[DEBUG] Calculated base value: {baseValueCalculated}");
        }
        else
        {
            baseValueCalculated = effect.baseValue;
            Debug.Log($"[DEBUG] No Scaling Base value: {baseValueCalculated}");
        }


        // Apply effects based on type
        switch (effect.effectType)
        {
            case BaseUnit.EffectType.Damage:
                ApplyDamage(target, attacker, effect.targetedStat, effect.scalingAttribute, baseValueCalculated); // Pass correct parameters
                break;
            case BaseUnit.EffectType.Heal:
                ApplyHealing(target, attacker, baseValueCalculated);
                break;
            case BaseUnit.EffectType.Buff:
                ModifyStat(target, effect.targetedStat, baseValueCalculated, false); // Buff = positive change
                break;
            case BaseUnit.EffectType.Debuff:
                ModifyStat(target, effect.targetedStat, baseValueCalculated, true); // Debuff = negative change
                break;
        }

        // Apply status effect if present
        if (effect.statusEffect != null)
        {
            ApplyStatusEffect(target, attacker, effect.statusEffect);
        }

        if (GetStat(target, "HP") <= 0)
        {
            if (skipFight == true)
            {
                RemoveUnit(target);  // Instantly remove the unit if skipping the fight
            }
            else
            {
                StartCoroutine(HandleDeathWithDelay(target));
            }
        }

        IEnumerator HandleDeathWithDelay(BaseUnit target)
        {

            yield return new WaitForSeconds(0.2f);  // Delay if not skipping
            RemoveUnit(target);

        }


    }
    void ApplyDamage(BaseUnit target, BaseUnit currentAttacker, string defenseStat, string attackStat, int baseValueCalculated)
    {
        int finalDamage = 0;

        if (defenseStat == "TRUE")
        {
            finalDamage = baseValueCalculated;
            Debug.Log("Dealing true damage. with base value calculated: " + baseValueCalculated + " to " + target.unitName + " with defense stat: " + defenseStat + " and attack stat: " + attackStat);
            int newHp = GetStat(target, "HP") - finalDamage;
            SetStat(target, "HP", newHp);
            UpdateHealth(target, newHp);
        }

        else if (defenseStat.Equals("%", System.StringComparison.OrdinalIgnoreCase))
        {
            // Deal damage as a percentage of the target's current HP
            int currentHp = GetStat(target, "HP");
            int percentDamage = Mathf.Max(Mathf.FloorToInt(currentHp * (baseValueCalculated / 100f)), 1); // Ensures at least 1 damage

            int newHp = currentHp - percentDamage;
            SetStat(target, "HP", newHp);
            UpdateHealth(target, newHp);


        }
        else
        {
            int attackValue = GetStat(currentAttacker, attackStat); // Attacker's stat (Sp. Atk or Atk)
            int defenseValue = GetStat(target, defenseStat); // Defender's stat (Sp. Def or Def)
            int movePower = baseValueCalculated;

            // Stat ratio and move power
            float statRatio = (float)attackValue / Mathf.Max(defenseValue, 1); // Avoid division by zero
            float baseDamage = (movePower * statRatio) / 2f;

            // Apply random variation (85% to 100%)
            float randomMultiplier = Random.Range(0.85f, 1.00f);
            finalDamage = Mathf.Max(Mathf.FloorToInt(baseDamage * randomMultiplier), 1);

            // Apply damage
            int newHp = GetStat(target, "HP") - finalDamage;
            SetStat(target, "HP", newHp);
            UpdateHealth(target, newHp);

        }

        if (!skipFight)
        {
            ShowEffectOnUnit(target, finalDamage.ToString(), Color.red, defenseStat, false);
        }

        target.UpdateDamageTaken(finalDamage);
        if (currentAttacker != null)
        {
            currentAttacker.UpdateDamageDealt(finalDamage);
        }
    }


    void ApplyHealing(BaseUnit target, BaseUnit attacker, int healAmount)
    {

        int newHp = Mathf.Min(GetStat(target, "HP") + healAmount, target.maxHp);
        SetStat(target, "HP", newHp);
        UpdateHealth(target, newHp);
        if (!skipFight)
            ShowEffectOnUnit(target, healAmount.ToString(), Color.green, "HP", false);

        attacker.UpdateHealingDone(healAmount);
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
        else if (statNameLower == "AURA" || statNameLower == "aura")
            unit.aura = newValue;
        else
            Debug.LogError($"Stat '{statName}' not found for {unit.unitName}!");
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
        if (statNameLower == "AURA" || statNameLower == "aura")
            Debug.Log("Aura: " + unit.aura + " for " + unit.unitName);
        return unit.aura;
    }
    void ApplyStatusEffect(BaseUnit target, BaseUnit attacker, BaseUnit.StatusEffect statusEffect)
    {
        if (statusEffect.ccType == BaseUnit.CrowdControlType.Poison)
        {
            if (statusEffect.isRng == true)
            {
                if (CalculateRngAsAura(attacker.aura))
                {
                    target.isPoisoned = true;
                    target.poisonDuration = statusEffect.duration;
                }
            }
            else
            {
                target.isPoisoned = true;
                target.poisonDuration = statusEffect.duration;
            }
        }

        else if (statusEffect.ccType == BaseUnit.CrowdControlType.Stun)
        {
            if (CalculateRngAsAura(attacker.aura))
            {
                target.isStunned = true;
                target.stunDuration = statusEffect.duration;
            }
        }

        else if (statusEffect.ccType == BaseUnit.CrowdControlType.Taunt)
        {
            if (statusEffect.isRng == true)
            {
                if (CalculateRngAsAura(attacker.aura))
                {
                    target.isTaunting = true;
                    target.tauntDuration = statusEffect.duration;
                }
            }
            else
            {
                target.isTaunting = true;
                target.tauntDuration = statusEffect.duration;
            }

        }
    }


    void CheckStatusEffects(BaseUnit unit)
    {
        if (unit.isPoisoned)
        {
            unit.baseHp -= unit.baseHp / 10; // Deal 10% of max HP as poison damage
            unit.poisonDuration--;

            if (unit.poisonDuration <= 0)
            {
                unit.isPoisoned = false;
            }
            UpdateHealth(unit, unit.baseHp);
            ShowEffectOnUnit(unit, (unit.baseHp / 10).ToString(), Color.red, "HP", false);
        }
        if (unit.isTaunting)
        {
            unit.tauntDuration--;
            if (unit.tauntDuration <= 0)
            {
                unit.isTaunting = false;
            }
        }
    }



    bool CalculateRngAsAura(float auraPercentage)
    {
        float rngValue = Random.Range(0f, 100f);
        return rngValue <= auraPercentage;
    }



    void RemoveUnit(BaseUnit unit)
    {
        if (playerUnits.Contains(unit))
        {
            playerUnits.Remove(unit);
            deadPlayerUnits.Add(unit);
        }
        else if (enemyUnits.Contains(unit))
        {
            enemyUnits.Remove(unit);
            deadEnemyUnits.Add(unit);
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
        if (menu != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                squadManager.SetSpriteLayerToDefault(deadPlayerUnits, deadEnemyUnits);
                GameObject instantiatedMenu = Instantiate(menu, canvas.transform);
                instantiatedMenu.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogError("Canvas not found in the scene.");
            }
        }
        else
        {
            Debug.LogError("Menu prefab not assigned in the inspector.");
        }
    }
}
