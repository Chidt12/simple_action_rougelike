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
            _subscriptions.Add(SimpleMessenger.Subscribe<SentHealMessage>(OnSentHeal));
            _subscriptions.Add(SimpleMessenger.Subscribe<SentDamageMessage>(OnSentDamage));
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
            var statData = message.Creator as IEntityStatData;
            if (statData != null)
            {
                var critChance = statData.GetTotalStatValue(StatType.CritChance);
                if (message.DamageSource != EffectSource.FromNormalAttack)
                    critChance = 0;

                var attackDamage = statData.GetTotalStatValue(StatType.AttackDamage);
                var isCrit = Random.Range(0, 1f) < critChance;
                var damageValue = attackDamage;
                if (message.DamageFactors != null && message.DamageFactors.Length > 0)
                {
                    float damageConfig = 0;
                    foreach (var damageFactor in message.DamageFactors)
                        damageConfig += (statData.GetTotalStatValue(damageFactor.damageFactorStatType) * damageFactor.damageFactorValue);
                    damageValue = damageConfig;
                }
                else
                {
                    damageValue = 0;
                }
                damageValue += message.DamageBonus;

                float critDamage = 0;
                if (isCrit)
                {
                    critDamage = statData.GetTotalStatValue(StatType.CritDamage);
                    damageValue = damageValue * (1 + critDamage);
                }

                var armorPenetration = statData.GetTotalStatValue(StatType.ArmorPenetration);
#if UNITY_EDITOR
                Debug.Log($"damage_log|| owner: {message.Creator.EntityId}/{message.Creator.EntityType} | damage source: {message.DamageSource}| attack damage: {attackDamage} | isCrit: {isCrit} | critDamage: {critDamage}" +
                      $" | armorPenet: {armorPenetration} | damageFactor: {(message.DamageFactors != null && message.DamageFactors.Length > 0 ? GetTextFactorLog(message.DamageFactors) : "1")} | damageBonus: {message.DamageBonus}");
#endif

                SimpleMessenger.Publish(new MessageToEntity(message.Target.EntityUID), new ReceivedDamageMessage());
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