using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Message;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class GameplayMessageCenter
    {
        private partial void OnSentDamage(SentDamageMessage message)
        {
            var targetModifiedStatData = message.Target as IEntityModifiedStatData;
            if (targetModifiedStatData == null)
            {
                return;
            }

            // Calculated Damage created

            var creatorStatData = message.Creator as IEntityStatData;
            float critChance = 0;
            float attackDamage = 0;
            float critDamage = 0;
            float armorPenetration = 0;
            float lifeSteal = 0;

            if (creatorStatData != null)
            {
                critChance = creatorStatData.GetTotalStatValue(StatType.CritChance);
                attackDamage = creatorStatData.GetTotalStatValue(StatType.AttackDamage);
                critDamage = creatorStatData.GetTotalStatValue(StatType.CritDamage);
                armorPenetration = creatorStatData.GetTotalStatValue(StatType.ArmorPenetration);
                lifeSteal = creatorStatData.GetTotalStatValue(StatType.LifeSteal);
            }

            var prepareDamageModifier = new PrepareDamageModifier(message.DamageProperty, message.DamageBonus, message.DamageFactors, critChance);
            if(message.Creator.EntityType == EntityType.Hero && preCalculateDamageModifiers != null)
            {
                foreach (var item in preCalculateDamageModifiers)
                    prepareDamageModifier = item.Calculate(message.Target, message.DamageSource, prepareDamageModifier);
            }

            var damageValue = prepareDamageModifier.damageBonus;
            if (creatorStatData != null && prepareDamageModifier.damageFactors != null && prepareDamageModifier.damageFactors.Length > 0)
            {
                float damageConfig = 0;
                foreach (var damageFactor in prepareDamageModifier.damageFactors)
                    damageConfig += (creatorStatData.GetTotalStatValue(damageFactor.damageFactorStatType) * damageFactor.damageFactorValue);
                damageValue += damageConfig;
            }

            if (message.DamageSource != EffectSource.FromNormalAttack)
                prepareDamageModifier.critChance = 0;
            var isCrit = prepareDamageModifier.critChance <= 0 ? false : Random.Range(0, 1f) < prepareDamageModifier.critChance;
            if (isCrit)
                damageValue = damageValue * (1 + critDamage);



#if UNITY_EDITOR
            Debug.Log($"damage_log|| owner: {message.Creator.EntityId}/{message.Creator.EntityType} | damage source: {message.DamageSource}| attack damage: {attackDamage} | isCrit: {isCrit} | critDamage: {critDamage}" +
                  $" | armorPenet: {armorPenetration} | damageFactor: {(prepareDamageModifier.damageFactors != null && prepareDamageModifier.damageFactors.Length > 0 ? GetTextFactorLog(prepareDamageModifier.damageFactors) : "1")} | damageBonus: {prepareDamageModifier.damageBonus}");
#endif
            var damageInfo = new DamageInfo(message.DamageSource, damageValue, message.Creator, message.Target, prepareDamageModifier.damageProperty);
            if (message.Creator.EntityType == EntityType.Hero && postCalculateDamageModifiers != null)
            {
                foreach (var item in postCalculateDamageModifiers)
                    damageInfo = item.Calculate(damageInfo);
            }

            // Calculate Damage Received

            float dodgeChance = 0;
            float armor = 0;
            float damageReduction = 0;

            if (targetModifiedStatData != null)
            {
                dodgeChance = targetModifiedStatData.GetTotalStatValue(StatType.DodgeChance);
                armor = targetModifiedStatData.GetTotalStatValue(StatType.Armor);
                damageReduction = targetModifiedStatData.GetTotalStatValue(StatType.DamageReduction);
            }

            if (Random.Range(0, 1f) >= dodgeChance)
            {
                var damageTaken = (damageInfo.damage - armor * (1 - armorPenetration)) * (1 - damageReduction);
                damageTaken = damageTaken > 0 ? damageTaken : 0;

                if (message.Target.EntityType == EntityType.Hero && damageModifiers != null)
                {
                    foreach (var item in damageModifiers)
                        damageTaken = item.Damage(damageTaken, damageInfo.damageSource, damageInfo.damageProperty, damageInfo.creatorData);
                }

#if UNITY_EDITOR
                var log = $"get_damage_log || target: {message.Target.EntityId}/{message.Target.EntityType} | damageReduction: {damageReduction} " +
                      $"| armor: {armor} | damageTaken: {damageTaken}";
                Debug.Log($"{log}");
#endif

                var finalCreatedDamage = targetModifiedStatData.GetDamage(damageTaken, damageInfo.damageSource, isCrit ? EffectProperty.Crit : damageInfo.damageProperty);

                if (message.Target.IsDead)
                {
                    var deathEntityData = message.Target as IEntityDeathData;
                    if (deathEntityData != null)
                        SimpleMessenger.Publish(new EntityDiedMessage(message.Target, deathEntityData.DeathDataIdentity));
                    else
                        SimpleMessenger.Publish(new EntityDiedMessage(message.Target, new DeathDataIdentity(0, DeathType.None)));
                }
            }
            else
            {
                message.Target.ReactionChangedEvent.Invoke(EntityReactionType.Dodge);
            }
        }

        private string GetTextFactorLog(DamageFactor[] damageFactors)
        {
            var textFactor = "";
            foreach (var damageFactor in damageFactors)
                textFactor += $"{damageFactor.damageFactorStatType} - {damageFactor.damageFactorValue}";
            return textFactor;
        }
    }

}