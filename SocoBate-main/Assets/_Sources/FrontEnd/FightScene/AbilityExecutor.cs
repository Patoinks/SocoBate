// AbilityExecutor.cs (Definitive, with Impact Callback)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class AbilityExecutor
{
    public static IEnumerator Execute(UnitFacade user, BaseUnit.AttackData ability)
    {
        if (ability == null || ability.effects == null || ability.effects.Count == 0)
        {
            yield break;
        }

        BaseUnit.Effect firstEffect = ability.effects.First();
        UnitFacade primaryTarget = TargetingSystem.FindTargets(user, firstEffect.targetType).FirstOrDefault();
        
        bool isTargetAnEnemy = primaryTarget != null && (firstEffect.targetType == BaseUnit.TargetType.SingleEnemy || firstEffect.targetType == BaseUnit.TargetType.AllEnemies);

        // --- THIS IS THE NEW LOGIC ---
        if (isTargetAnEnemy)
        {
            // 1. Define what should happen AT THE MOMENT OF IMPACT.
            // This 'Action' packages up all the effect logic.
            System.Action impactAction = () => 
            {
                foreach (var effect in ability.effects)
                {
                    List<UnitFacade> targets = TargetingSystem.FindTargets(user, effect.targetType);
                    if (targets == null || targets.Count == 0) continue;
                    foreach (var target in targets)
                    {
                        if (target == null || target.UnitData.baseHp <= 0) continue;
                        EffectExecutor.ApplyEffect(effect, user, target);
                    }
                }
            };
            
            // 2. Pass this action to the animation controller.
            // The controller is now responsible for deciding WHEN to execute it.
            yield return UnitAnimationController.Instance.AnimateMeleeAttack(user, primaryTarget, 0.2f, impactAction);
        }
        else // This handles buffs, self-heals, etc., that don't need an impact animation.
        {
            // Apply the logic instantly without an animation.
            foreach (var effect in ability.effects)
            {
                List<UnitFacade> targets = TargetingSystem.FindTargets(user, effect.targetType);
                if (targets == null || targets.Count == 0) continue;
                foreach (var target in targets)
                {
                    if (target == null || target.UnitData.baseHp <= 0) continue;
                    EffectExecutor.ApplyEffect(effect, user, target);
                }
            }
            // Wait for a moment to give the effect some visual time.
            yield return new WaitForSeconds(0.3f);
        }
    }

    public static void ExecuteSynchronous(UnitFacade user, BaseUnit.AttackData ability)
    {
        if (ability == null || ability.effects == null || ability.effects.Count == 0) return;
        foreach (var effect in ability.effects)
        {
            List<UnitFacade> targets = TargetingSystem.FindTargets(user, effect.targetType);
            if (targets == null || targets.Count == 0) continue;
            foreach (var target in targets)
            {
                if (target == null || target.UnitData.baseHp <= 0) continue;
                EffectExecutor.ApplyEffect(effect, user, target);
            }
        }
    }
}