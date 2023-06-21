using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Manager.Gameplay;
using Runtime.Message;

namespace Runtime.Gameplay.EntitySystem
{
    public class CauseStatusAfterHitsShopInGameItem : ShopInGameItem<CauseStatusAfterHitsShopInGameDataConfigItem>, IFinalDamagedModifier
    {
        private int _currentHitNumber;

        protected override void Apply()
        {
            _currentHitNumber = 0;
            GameplayManager.Instance.MessageCenter.AddFinalDamageCreatedModifier(this);
        }

        public override void Remove()
        {
            GameplayManager.Instance.MessageCenter.RemoveFinalDamageCreatedModifier(this);
        }

        public void Finalize(float damageCreated, IEntityData receiver)
        {
            if(damageCreated > 0)
            {
                _currentHitNumber++;
            }

            if(_currentHitNumber >= dataConfigItem.numberHitTriggered)
            {
                _currentHitNumber = 0;
                var receiverStatusData = receiver as IEntityStatusData;
                if (receiverStatusData != null)
                {
                    var direction = receiver.Position - owner.Position;
                    var statusMetaData = new StatusMetaData(direction, owner.Position);
                    SimpleMessenger.Publish(MessageScope.EntityMessage, new SentStatusEffectMessage(owner, receiverStatusData, dataConfigItem.statusIdentity, statusMetaData));
                }
            }
        }
    }
}
