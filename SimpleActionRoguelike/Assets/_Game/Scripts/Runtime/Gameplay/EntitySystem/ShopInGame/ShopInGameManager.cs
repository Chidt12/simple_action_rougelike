using Cysharp.Threading.Tasks;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Manager.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShopInGameManager : MonoSingleton<ShopInGameManager>
    {
        private List<ShopInGameItem> _shopInGameItems;

        protected override void Awake()
        {
            base.Awake();
            _shopInGameItems = new();
        }

        public async UniTask AddShopInGameItem(IEntityData ownerData, ShopInGameItemType shopInGameItemType, int dataId)
        {
            var dataConfigItem = await DataManager.Config.LoadShopInGameDataConfigItem(shopInGameItemType, dataId);
            var shopInGameItem = ShopInGameItemFactory.GetShopInGameItem(shopInGameItemType);
            shopInGameItem.Apply(ownerData, dataConfigItem);
        }

        public void RemoveShopInGameItem(ShopInGameItem shopInGameItem)
        {
            shopInGameItem.Remove();
            _shopInGameItems.Remove(shopInGameItem);
        }
    }
}