using Runtime.ConfigModel;
using Runtime.Manager.Gameplay;

namespace Runtime.Gameplay.EntitySystem
{
    public class HealAfterCritShopInGameItem : ShopInGameItem<HealAfterCritShopInGameDataConfigItem>, IFinalDamagedModifier
    {
        public int Priority => 0;

        public override void Remove()
        {
            GameplayManager.Instance.MessageCenter.RemoveFinalDamageCreatedModifier(this);
        }

        protected override void Apply()
        {
            GameplayManager.Instance.MessageCenter.AddFinalDamageCreatedModifier(this);
        }

        public void Finalize(float damageCreated, EffectSource effectSource, EffectProperty effectProperty, IEntityData receiver)
        {
            if (damageCreated > 0 && effectProperty == EffectProperty.Crit)
            {
                var statData = owner as IEntityModifiedStatData;
                if(statData != null)
                {
                    statData.Heal(dataConfigItem.healAmount, EffectSource.FromArtifact, EffectProperty.Normal);
                }
            }
        }
    }
}