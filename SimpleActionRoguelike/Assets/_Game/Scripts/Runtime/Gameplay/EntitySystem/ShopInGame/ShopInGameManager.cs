using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Manager.Data;
using System.Collections.Generic;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShopInGameManager
    {
        private List<ShopInGameItem> _shopInGameItems;

        public List<ShopInGameItem> CurrentShopInGameItems => _shopInGameItems;

        public ShopInGameManager()
        {

        }

        public void Init()
        {
            _shopInGameItems = new();
        }

        public void Dispose()
        {
            foreach (var item in _shopInGameItems)
            {
                item.Remove();
            }

            _shopInGameItems.Clear();
        }

        public async UniTask AddShopInGameItem(IEntityModifiedStatData ownerData, ShopInGameItemType shopInGameItemType, int dataId)
        {
            var dataConfigItem = await DataManager.Config.LoadShopInGameDataConfigItem(shopInGameItemType, dataId);
            var shopInGameItem = ShopInGameItemFactory.GetShopInGameItem(shopInGameItemType);
            shopInGameItem.Apply(ownerData, dataConfigItem);
            _shopInGameItems.Add(shopInGameItem);
        }

        public void RemoveShopInGameItem(ShopInGameItem shopInGameItem)
        {
            shopInGameItem.Remove();
            _shopInGameItems.Remove(shopInGameItem);
        }
    }
}