using Runtime.ConfigModel;
using Runtime.Core.Message;
using ZBase.Foundation.PubSub;
using Runtime.Message;
using Runtime.Manager.Gameplay;

namespace Runtime.Gameplay.EntitySystem
{
    public class GetMoreCoinsWhenGoShopShopInGameItem : ShopInGameItem<GetMoreCoinsWhenGoShopShopInGameDataConfigItem>
    {
        private ISubscription _subscription;

        public override void Remove()
        {
            _subscription.Dispose();
        }

        protected override void Apply()
        {
            _subscription = SimpleMessenger.Subscribe<EnteredNextLevelMessage>(OnEnteredNextLevel);
        }

        private void OnEnteredNextLevel(EnteredNextLevelMessage message)
        {
            if(message.RoomType == Definition.GameplayRoomType.Shop)
            {
                GameplayManager.Instance.GiveRewards(dataConfigItem.numberOfCoins);
            }
        }
    }
}