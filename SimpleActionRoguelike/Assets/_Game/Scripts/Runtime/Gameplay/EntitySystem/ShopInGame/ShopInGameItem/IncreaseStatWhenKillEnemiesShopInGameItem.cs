using Runtime.ConfigModel;
using Runtime.Manager.Gameplay;

namespace Runtime.Gameplay.EntitySystem
{
    public class IncreaseStatWhenKillEnemiesShopInGameItem : ShopInGameItem<IncreaseStatWhenKillEnemiesShopInGameDataConfigItem>, IFinalDamagedModifier
    {
        private float _currentBuffedValue;
        private int _currentCountEnemies;

        public int Priority => 0;

        public override void Remove()
        {
            GameplayManager.Instance.MessageCenter.RemoveFinalDamageCreatedModifier(this);
            if(owner != null)
            {
                owner.DebuffStat(dataConfigItem.statType, _currentBuffedValue, Definition.StatModifyType.TotalBonus);
            }
        }

        protected override void Apply()
        {
            _currentBuffedValue = 0;
            GameplayManager.Instance.MessageCenter.AddFinalDamageCreatedModifier(this);
        }

        public void Finalize(float damageCreated, EffectSource effectSource, EffectProperty effectProperty, IEntityData receiver)
        {
            if(receiver != null && receiver.IsDead && receiver.EntityType.IsEnemy())
            {
                _currentCountEnemies++;
                if(_currentBuffedValue < dataConfigItem.maxValue && _currentCountEnemies >= dataConfigItem.numberEnemiesToIncrease)
                {
                    _currentCountEnemies = 0;
                    if (owner != null)
                    {
                        owner.BuffStat(dataConfigItem.statType, dataConfigItem.valueEachTurn, Definition.StatModifyType.TotalBonus);
                        _currentBuffedValue += dataConfigItem.valueEachTurn;
                    }
                }
            }
        }
    }
}