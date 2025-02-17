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
            (name, base_hp, base_def, base_speed, base_str, base_int, base_evasion, base_luck, rarity)
            VALUES (@name, @hp, @def, @speed, @str, @int, @evasion, @luck, @rarity)
            ON DUPLICATE KEY UPDATE 
            base_hp = COALESCE(@hp, base_hp),
            base_def = COALESCE(@def, base_def),
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
                { "@def", unit.baseDef },
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
            string query = @"
            INSERT INTO PassiveAbilities 
            (unit_name, effect_type, scaling_attribute, scaling_percent, value, is_aoe, is_percentage, is_cc, cc_duration, targeted_stat, description)
            VALUES (@unit_name, @effect, @scalingAttr, @scalingPercent, @value, @isAoE, @isPercentage, @isCC, @ccDuration, @targetedStat, @description)
            ON DUPLICATE KEY UPDATE 
            effect_type = COALESCE(@effect, effect_type),
            scaling_attribute = COALESCE(@scalingAttr, scaling_attribute),
            scaling_percent = COALESCE(@scalingPercent, scaling_percent),
            value = COALESCE(@value, value),
            is_aoe = COALESCE(@isAoE, is_aoe),
            is_percentage = COALESCE(@isPercentage, is_percentage),
            is_cc = COALESCE(@isCC, is_cc),
            cc_duration = COALESCE(@ccDuration, cc_duration),
            targeted_stat = COALESCE(@targetedStat, targeted_stat),
            description = COALESCE(@description, description);";

            var parameters = new Dictionary<string, object>
            {
                { "@unit_name", unit.unitName },
                { "@effect", unit.passiveEffectType },
                { "@scalingAttr", unit.passiveScalingAttribute },
                { "@scalingPercent", unit.passiveScalingPercent },
                { "@value", unit.passiveValue },
                { "@isAoE", unit.isPassiveAoE },
                { "@isPercentage", unit.isPassivePercentage },
                { "@isCC", unit.isPassiveCC },
                { "@ccDuration", unit.passiveCCDuration },
                { "@targetedStat", unit.passiveTargetedStat },
                { "@description", unit.passiveDescription }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);
            return result != null;
        }

        public async static Task<bool> InsertOrUpdateSpecialAttack(BaseUnit unit)
        {
            string query = @"
            INSERT INTO SpecialAttacks 
            (unit_name, effect_type, scaling_attribute, scaling_percent, value, is_aoe, is_percentage, is_cc, cc_duration, targeted_stat, turns_to_special, description)
            VALUES (@unit_name, @effect, @scalingAttr, @scalingPercent, @value, @isAoE, @isPercentage, @isCC, @ccDuration, @targetedStat, @turns, @description)
            ON DUPLICATE KEY UPDATE 
            effect_type = COALESCE(@effect, effect_type),
            scaling_attribute = COALESCE(@scalingAttr, scaling_attribute),
            scaling_percent = COALESCE(@scalingPercent, scaling_percent),
            value = COALESCE(@value, value),
            is_aoe = COALESCE(@isAoE, is_aoe),
            is_percentage = COALESCE(@isPercentage, is_percentage),
            is_cc = COALESCE(@isCC, is_cc),
            cc_duration = COALESCE(@ccDuration, cc_duration),
            targeted_stat = COALESCE(@targetedStat, targeted_stat),
            turns_to_special = COALESCE(@turns, turns_to_special),
            description = COALESCE(@description, description);";

            var parameters = new Dictionary<string, object>
            {
                { "@unit_name", unit.unitName },
                { "@effect", unit.specialEffectType },
                { "@scalingAttr", unit.specialScalingAttribute },
                { "@scalingPercent", unit.specialScalingPercent },
                { "@value", unit.specialValue },
                { "@isAoE", unit.isSpecialAttackAoE },
                { "@isPercentage", unit.isSpecialAttackPercentage },
                { "@isCC", unit.isSpecialAttackCC },
                { "@ccDuration", unit.specialAttackCCDuration },
                { "@targetedStat", unit.specialTargetedStat },
                { "@turns", unit.turnsToSpecial },
                { "@description", unit.specialDescription }
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
