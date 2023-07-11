using Runtime.ConfigModel;
using Runtime.Manager.Gameplay;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class HealWhenDefeatEnemyInStatusShopInGameItem : ShopInGameItem<HealWhenDefeatEnemyInStatusShopInGameDataConfigItem>, IFinalDamagedModifier
    {
        public int Priority => 0;

        protected override void Apply()
        {
            GameplayManager.Instance.MessageCenter.AddFinalDamageCreatedModifier(this);
        }

        public override void Remove()
        {
            GameplayManager.Instance.MessageCenter.RemoveFinalDamageCreatedModifier(this);
        }

        public void Finalize(float damageCreated, EffectSource effectSource, EffectProperty effectProperty, IEntityData receiver)
        {
            if(receiver.IsDead && damageCreated > 0)
            {
                var statusData = receiver as IEntityStatusData;
                if(statusData != null)
                {
                    if(statusData.CheckContainStatusInStack(new[] { dataConfigItem.triggeredStatusType }))
                    {
                        owner.Heal(dataConfigItem.healAmount, EffectSource.None, EffectProperty.Normal);
                    }
                }
            }
        }
    }

}