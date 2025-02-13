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
        public async static Task<bool> InsertUnit(BaseUnit unit)
        {
            string query = @"
            INSERT INTO Units 
            (name, base_hp, base_def, base_speed, base_str, base_int, base_evasion, base_luck, rarity)
            VALUES (@name, @hp, @def, @speed, @str, @int, @evasion, @luck, @rarity)";

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

        public async static Task<bool> InsertPassive(BaseUnit unit)
        {
            string query = @"
    INSERT INTO PassiveAbilities 
    (unit_name, effect_type, scaling_attribute, scaling_percent, value, is_aoe, is_percentage, is_cc, cc_duration, targeted_stat, description) 
    VALUES 
    (@unit_name, @effect, @scalingAttr, @scalingPercent, @value, @isAoE, @isPercentage, @isCC, @ccDuration, @targetedStat, @description)";

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
        { "@description", unit.passiveDescription } // Added description
    };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);
            return result != null;
        }


        public async static Task<bool> InsertAttack(BaseUnit unit)
        {
            string query = @"
    INSERT INTO NormalAttacks 
    (unit_name, effect_type, scaling_attribute, scaling_percent, value, is_aoe, is_percentage, is_cc, cc_duration, targeted_stat, description) 
    VALUES (@unit_name, @effect, @scalingAttr, @scalingPercent, @value, @isAoE, @isPercentage, @isCC, @ccDuration, @targetedStat, @description)";

            var parameters = new Dictionary<string, object>
    {
        { "@unit_name", unit.unitName },
        { "@effect", unit.normalAttackEffectType },
        { "@scalingAttr", unit.normalAttackScalingAttribute }, // Adjust if needed
        { "@scalingPercent", unit.normalAttackScalingPercent }, // Adjust if needed
        { "@value", unit.normalAttackValue },
        { "@isAoE", unit.isNormalAttackAoE },
        { "@isPercentage", unit.isNormalAttackPercentage }, // Adjust if needed
        { "@isCC", unit.isNormalAttackCC },
        { "@ccDuration", unit.normalAttackCCDuration },
        { "@targetedStat", unit.normalAttackTargetedStat }, // Adjust if needed
        { "@description", unit.normalAttackDescription }  // Add description field
    };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);
            return result != null;
        }


        public async static Task<bool> InsertSpecialAttack(BaseUnit unit)
        {
            string query = @"
    INSERT INTO SpecialAttacks 
    (unit_name, effect_type, scaling_attribute, scaling_percent, value, is_aoe, is_percentage, is_cc, cc_duration, targeted_stat, turns_to_special, description)
    VALUES (@unit_name, @effect, @scalingAttr, @scalingPercent, @value, @isAoE, @isPercentage, @isCC, @ccDuration, @targetedStat, @turns, @description)";

            var parameters = new Dictionary<string, object>
    {
        { "@unit_name", unit.unitName },
        { "@effect", unit.specialEffectType },
        { "@scalingAttr", unit.specialScalingAttribute }, // Adjust if needed
        { "@scalingPercent", unit.specialScalingPercent }, // Adjust if needed
        { "@value", unit.specialValue },
        { "@isAoE", unit.isSpecialAttackAoE },
        { "@isPercentage", unit.isSpecialAttackPercentage }, // Adjust if needed
        { "@isCC", unit.isSpecialAttackCC },
        { "@ccDuration", unit.specialAttackCCDuration }, // Adjust if needed
        { "@targetedStat", unit.specialTargetedStat }, // Adjust if needed
        { "@turns", unit.turnsToSpecial },
        { "@description", unit.specialDescription }  // Add description field
    };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);
            return result != null;
        }

        public async static Task<bool> InsertUnitWithAllData(BaseUnit unit)
        {
            bool success = await InsertUnit(unit);
            success &= await InsertPassive(unit);
            //success &= await InsertAttack(unit);
            success &= await InsertSpecialAttack(unit);

            return success;
        }
    }
}
