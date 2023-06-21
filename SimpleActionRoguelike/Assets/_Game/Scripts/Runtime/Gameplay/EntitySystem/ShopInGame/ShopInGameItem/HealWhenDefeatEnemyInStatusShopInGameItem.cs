using Runtime.ConfigModel;
using Runtime.Manager.Gameplay;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class HealWhenDefeatEnemyInStatusShopInGameItem : ShopInGameItem<HealWhenDefeatEnemyInStatusShopInGameDataConfigItem>, IFinalDamagedModifier
    {
        protected override void Apply()
        {
            GameplayManager.Instance.MessageCenter.AddFinalDamageCreatedModifier(this);
        }

        public override void Remove()
        {
            GameplayManager.Instance.MessageCenter.RemoveFinalDamageCreatedModifier(this);
        }

        public void Finalize(float damageCreated, IEntityData receiver)
        {
            if(receiver.IsDead && damageCreated > 0)
            {
                var statusData = receiver as IEntityStatusData;
                if(statusData != null)
                {
                    if(statusData.CheckContainStatusInStack(new[] { dataConfigItem.triggeredStatusType }))
                    {
                        var modifiedStatData = owner as IEntityModifiedStatData;
                        if(modifiedStatData != null)
                        {
                            modifiedStatData.Heal(dataConfigItem.healAmount, EffectSource.None, EffectProperty.Normal);
                        }
                    }
                }
            }
        }
    }

}