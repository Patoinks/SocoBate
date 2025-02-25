using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MySql.Data.MySqlClient;
using Context;
using Models;

namespace Database
{
    public static class UnitCreator
    {
        public async static Task<bool> InsertOrUpdateUnit(BaseUnit unit)
        {
            string query = @"
            INSERT INTO Units 
                (name, base_hp, m_def, p_def, base_speed, base_str, base_int, base_evasion, base_luck, rarity)
            VALUES 
                (@name, @hp, @mDef, @pDef, @speed, @str, @int, @evasion, @luck, @rarity)
            ON DUPLICATE KEY UPDATE 
                base_hp = COALESCE(@hp, base_hp),
                m_def = COALESCE(@mDef, m_def),
                p_def = COALESCE(@pDef, p_def),
                base_speed = COALESCE(@speed, base_speed),
                base_str = COALESCE(@str, base_str),
                base_int = COALESCE(@int, base_int),
                base_evasion = COALESCE(@evasion, base_evasion),
                base_luck = COALESCE(@luck, base_luck),
                rarity = COALESCE(@rarity, rarity);";

            var parameters = new Dictionary<string, object>
            {
                { "@name", unit.unitName },
                { "@hp", unit.baseHp },
                { "@mDef", unit.mDef }, // Changed baseDef to mDef
                { "@pDef", unit.pDef }, // Changed baseDef to pDef
                { "@speed", unit.baseSpeed },
                { "@str", unit.baseStr },
                { "@int", unit.baseInt },
                { "@evasion", unit.baseEvasion },
                { "@luck", unit.baseLuck },
                { "@rarity", unit.rarity }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);
            return result != null;
        }

        public async static Task<bool> InsertOrUpdatePassive(BaseUnit unit)
        {
            if (unit.passiveAbility == null || unit.passiveAbility.effects == null || unit.passiveAbility.effects.Count == 0)
                return true; // No passive ability, consider it a success.

            var effect = unit.passiveAbility.effects[0];
            string ccType = effect.statusEffect != null ? effect.statusEffect.ccType.ToString() : null;
            int? ccDuration = effect.statusEffect?.duration;

            string query = @"
            INSERT INTO PassiveAbilities 
            (unit_name, effect_type, target_type, targeted_stat, scaling_attribute, scaling_percent, base_value, is_percentage, cc_type, cc_duration, description)
            VALUES 
            (@unit_name, @effect, @targetType, @targetedStat, @scalingAttr, @scalingPercent, @value, @isPercentage, @ccType, @ccDuration, @description)
            ON DUPLICATE KEY UPDATE 
                effect_type = COALESCE(@effect, effect_type),
                target_type = COALESCE(@targetType, target_type),
                targeted_stat = COALESCE(@targetedStat, targeted_stat),
                scaling_attribute = COALESCE(@scalingAttr, scaling_attribute),
                scaling_percent = COALESCE(@scalingPercent, scaling_percent),
                base_value = COALESCE(@value, base_value),
                is_percentage = COALESCE(@isPercentage, is_percentage),
                cc_type = COALESCE(@ccType, cc_type),
                cc_duration = COALESCE(@ccDuration, cc_duration),
                description = COALESCE(@description, description);";

            var parameters = new Dictionary<string, object>
            {
                { "@unit_name", unit.unitName },
                { "@effect", effect.effectType.ToString() },
                { "@targetType", effect.targetType.ToString() },
                { "@targetedStat", effect.targetedStat },
                { "@scalingAttr", effect.scalingAttribute },
                { "@scalingPercent", effect.scalingPercent },
                { "@value", effect.baseValue },
                { "@isPercentage", effect.isPercentage },
                { "@ccType", ccType },
                { "@ccDuration", ccDuration },
                { "@description", unit.passiveAbility.description }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);
            return result != null;
        }

        public async static Task<bool> InsertOrUpdateSpecialAttack(BaseUnit unit)
        {
            if (unit.specialAttack == null || unit.specialAttack.effects == null || unit.specialAttack.effects.Count == 0)
                return true; // No special attack, consider it a success.

            var effect = unit.specialAttack.effects[0];
            string ccType = effect.statusEffect != null ? effect.statusEffect.ccType.ToString() : null;
            int? ccDuration = effect.statusEffect?.duration;

            string query = @"
            INSERT INTO SpecialAttacks 
            (unit_name, effect_type, target_type, targeted_stat, scaling_attribute, scaling_percent, base_value, is_percentage, cc_type, cc_duration, turns_to_special, description)
            VALUES 
            (@unit_name, @effect, @targetType, @targetedStat, @scalingAttr, @scalingPercent, @value, @isPercentage, @ccType, @ccDuration, @turns, @description)
            ON DUPLICATE KEY UPDATE 
                effect_type = COALESCE(@effect, effect_type),
                target_type = COALESCE(@targetType, target_type),
                targeted_stat = COALESCE(@targetedStat, targeted_stat),
                scaling_attribute = COALESCE(@scalingAttr, scaling_attribute),
                scaling_percent = COALESCE(@scalingPercent, scaling_percent),
                base_value = COALESCE(@value, base_value),
                is_percentage = COALESCE(@isPercentage, is_percentage),
                cc_type = COALESCE(@ccType, cc_type),
                cc_duration = COALESCE(@ccDuration, cc_duration),
                turns_to_special = COALESCE(@turns, turns_to_special),
                description = COALESCE(@description, description);";

            var parameters = new Dictionary<string, object>
            {
                { "@unit_name", unit.unitName },
                { "@effect", effect.effectType.ToString() },
                { "@targetType", effect.targetType.ToString() },
                { "@targetedStat", effect.targetedStat },
                { "@scalingAttr", effect.scalingAttribute },
                { "@scalingPercent", effect.scalingPercent },
                { "@value", effect.baseValue },
                { "@isPercentage", effect.isPercentage },
                { "@ccType", ccType },
                { "@ccDuration", ccDuration },
                { "@turns", unit.specialAttack.turnsToSpecial },
                { "@description", unit.specialAttack.description }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);
            return result != null;
        }

        public async static Task<bool> InsertOrUpdateUnitWithAllData(BaseUnit unit)
        {
            bool success = await InsertOrUpdateUnit(unit);
            success &= await InsertOrUpdatePassive(unit);
            success &= await InsertOrUpdateSpecialAttack(unit);
            return success;
        }
    }
}
