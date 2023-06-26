using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Runtime.Definition;
using System;
using Runtime.Core.Pool;
using Runtime.Constants;
using Runtime.Manager.Data;

namespace Runtime.UI
{
    public class InventoryShopItemUI : MonoBehaviour, IInventoryItem
    {
        [SerializeField] private CustomButton _clickButton;
        [SerializeField] private Image _icon;

        private Action<string> _loadInfoAction;
        private ShopInGameItemType _shopInGameItemType;
        private int _dataId;
        private bool _hasData;

        public async UniTask LoadUI(ShopInGameItemType shopItemType, int dataId, Action<string> loadInfoAction)
        {
            _hasData = true;
            _dataId = dataId;
            _shopInGameItemType = shopItemType;
            _icon.gameObject.SetActive(true);
            _loadInfoAction = loadInfoAction;
            _icon.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"shop_item_icon_{(int)shopItemType}_{dataId}"), this.GetCancellationTokenOnDestroy());
        }

        public void ClearUI(Action<string> loadInfoAction)
        {
            _hasData = false;
            _loadInfoAction = loadInfoAction;
            _icon.gameObject.SetActive(false);
        }

        public void ToggleSelect(bool value)
        {
            _clickButton.ToggleSelect(value);
            if (value)
            {
                if (_hasData)
                    LoadDescriptionAsync().Forget();
                else
                    _loadInfoAction?.Invoke(string.Empty);
            }
        }

        private async UniTaskVoid LoadDescriptionAsync()
        {
            var shopItem = await DataManager.Config.LoadShopInGameDataConfig(_shopInGameItemType);
            var description = await shopItem.GetDescription(_dataId);
            _loadInfoAction?.Invoke(description.Item2);
        }
    }
}