using Runtime.ConfigModel;
using Runtime.Manager.Gameplay;

namespace Runtime.Gameplay.EntitySystem
{
    public class CritAfterHitsShopInGameItem : ShopInGameItem<CritAfterHitsShopInGameDataConfigItem>, IFinalDamagedModifier, IPreCalculateDamageModifier
    {
        public int Priority => 0;
        private int _currentCritHits;

        public override void Remove()
        {
            GameplayManager.Instance.MessageCenter.RemoveFinalDamageCreatedModifier(this);
            GameplayManager.Instance.MessageCenter.RemovePreCalculateDamageModifier(this);
        }

        protected override void Apply()
        {
            GameplayManager.Instance.MessageCenter.AddFinalDamageCreatedModifier(this);
            GameplayManager.Instance.MessageCenter.AddPreCalculateDamageModifier(this);
        }

        public void Finalize(float damageCreated, EffectSource effectSource, EffectProperty effectProperty, IEntityData receiver)
        {
            if(damageCreated > 0)
            {
                _currentCritHits++;   
            }
        }

        public PrepareDamageModifier Calculate(IEntityData target, EffectSource damageSource, PrepareDamageModifier prepareDamageModifier)
        {
            if (_currentCritHits + 1 >= dataConfigItem.numberOfHit)
            {
                _currentCritHits = 0;
                prepareDamageModifier.critChance = 1;
            }
            return prepareDamageModifier;
        }
    }
}
