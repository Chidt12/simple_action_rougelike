using Runtime.Core.Message;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Message;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public class MessageCenter : MonoSingleton<MessageCenter>
    {
        private List<ISubscription> _subscriptions;

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentHealMessage>(OnSentHeal));
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentDamageMessage>(OnSentDamage));
        }

        private void OnDestroy()
        {
            foreach (var item in _subscriptions)
                item.Dispose();
        }

        #endregion API Methods

        #region Class Methods

        private void OnSentHeal(SentHealMessage message)
        {

        }

        private void OnSentDamage(SentDamageMessage message)
        {
            var targetModifiedStatData = message.Target as IEntityModifiedStatData;
            if(targetModifiedStatData == null)
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

            var damageValue = message.DamageBonus;
            if (creatorStatData != null && message.DamageFactors != null && message.DamageFactors.Length > 0)
            {
                float damageConfig = 0;
                foreach (var damageFactor in message.DamageFactors)
                    damageConfig += (creatorStatData.GetTotalStatValue(damageFactor.damageFactorStatType) * damageFactor.damageFactorValue);
                damageValue += damageConfig;
            }

            if (message.DamageSource != EffectSource.FromNormalAttack)
                critChance = 0;
            var isCrit = critChance <= 0 ? false : Random.Range(0, 1f) < critChance;
            if (isCrit)
                damageValue = damageValue * (1 + critDamage);

#if UNITY_EDITOR
            Debug.Log($"damage_log|| owner: {message.Creator.EntityId}/{message.Creator.EntityType} | damage source: {message.DamageSource}| attack damage: {attackDamage} | isCrit: {isCrit} | critDamage: {critDamage}" +
                  $" | armorPenet: {armorPenetration} | damageFactor: {(message.DamageFactors != null && message.DamageFactors.Length > 0 ? GetTextFactorLog(message.DamageFactors) : "1")} | damageBonus: {message.DamageBonus}");
#endif


            // Calculate Damage Received

            float dodgeChance = 0;
            float armor = 0;
            float damageReduction = 0;

            if(targetModifiedStatData != null)
            {
                dodgeChance = targetModifiedStatData.GetTotalStatValue(StatType.DodgeChance);
                armor = targetModifiedStatData.GetTotalStatValue(StatType.Armor);
                damageReduction = targetModifiedStatData.GetTotalStatValue(StatType.DamageReduction);
            }

            if (Random.Range(0, 1f) >= dodgeChance)
            {
                var damageTaken = (damageValue - armor * (1 - armorPenetration)) * (1 - damageReduction);
                damageTaken = damageTaken > 0 ? damageTaken : 0;

#if UNITY_EDITOR
                var log = $"get_damage_log || target: {message.Target.EntityId}/{message.Target.EntityType} | damageReduction: {damageReduction} " +
                      $"| armor: {armor} | damageTaken: {damageTaken}";
                Debug.Log($"{log}");
#endif

                var finalCreatedDamage = targetModifiedStatData.GetDamage(damageTaken, message.DamageSource, isCrit ? EffectProperty.Crit : message.DamageProperty);

                // TODO: Apply lifesteal and spawn sthing after death.
                if (message.Target.IsDead)
                    SimpleMessenger.Publish(new EntityDiedMessage(message.Target, false));
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

        #endregion Class Methods
    }
}