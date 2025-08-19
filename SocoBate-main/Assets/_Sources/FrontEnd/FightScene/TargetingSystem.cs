// TargetingSystem.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetingSystem
{
    public static List<UnitFacade> FindTargets(UnitFacade user, BaseUnit.TargetType targetType)
    {
        var playerUnits = UnitRegistry.Instance.GetPlayerUnits().Where(u => u.UnitData.baseHp > 0).ToList();
        var enemyUnits = UnitRegistry.Instance.GetEnemyUnits().Where(u => u.UnitData.baseHp > 0).ToList();

        List<UnitFacade> allies = user.IsEnemy ? enemyUnits : playerUnits;
        List<UnitFacade> opponents = user.IsEnemy ? playerUnits : enemyUnits;

        if (targetType == BaseUnit.TargetType.SingleEnemy || targetType == BaseUnit.TargetType.AllEnemies)
        {
            var tauntingEnemies = opponents.Where(u => u.UnitData.isTaunting).ToList();
            if (tauntingEnemies.Count > 0)
            {
                opponents = tauntingEnemies;
            }
        }
        
        switch (targetType)
        {
            case BaseUnit.TargetType.Self:
                return new List<UnitFacade> { user };

            case BaseUnit.TargetType.SingleEnemy:
            {
                // --- THIS IS THE FIX ---
                var frontRowHexes = new List<int> { 3, 6, 9 };
                var frontRowEnemies = opponents.Where(u => frontRowHexes.Contains(u.UnitData.HexId)).ToList();
                
                // First, try to find a random unit from the priority list (front row, or all taunters).
                UnitFacade target = GetRandomUnit(frontRowEnemies.Count > 0 ? frontRowEnemies : opponents);
                
                // If a valid target was found, return a list containing just that target.
                if (target != null)
                {
                    return new List<UnitFacade> { target };
                }
                // Otherwise, return an EMPTY list. Do NOT return a list with a null element.
                return new List<UnitFacade>();
            }

            case BaseUnit.TargetType.AllEnemies:
                return opponents;
                
            case BaseUnit.TargetType.SingleAlly:
            {
                // Applied the same fix here for safety.
                UnitFacade target = GetRandomUnit(allies.Where(u => u != user).ToList());
                if (target != null)
                {
                    return new List<UnitFacade> { target };
                }
                return new List<UnitFacade>();
            }

            case BaseUnit.TargetType.AllAllies:
                return allies;

            default:
                return new List<UnitFacade>();
        }
    }

    private static UnitFacade GetRandomUnit(List<UnitFacade> units)
    {
        if (units == null || units.Count == 0) return null;
        return units[Random.Range(0, units.Count)];
    }
}