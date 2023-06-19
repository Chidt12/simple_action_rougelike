using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Runtime.Definition;
using System;
using Runtime.Core.Pool;
using Runtime.Constants;

namespace Runtime.UI
{
    public class InventoryShopItemUI : MonoBehaviour, InventoryItem
    {
        [SerializeField] private CustomButton _clickButton;
        [SerializeField] private Image _icon;

        private Action<string> _loadInfoAction;

        public async UniTask LoadUI(ShopInGameItemType shopItemType, int dataId, Action<string> loadInfoAction)
        {
            _icon.gameObject.SetActive(true);
            _loadInfoAction = loadInfoAction;
            _icon.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"shop_item_icon_{(int)shopItemType}_{dataId}"), this.GetCancellationTokenOnDestroy());
        }

        public void ClearUI(Action<string> loadInfoAction)
        {
            _loadInfoAction = loadInfoAction;
            _icon.gameObject.SetActive(false);
        }
    }
}