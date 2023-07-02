using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Manager.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ShopInGameItemUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _price;
        [SerializeField] private Button _buyButton;

        public async UniTask Init(ShopInGameStageLoadConfigItem dataConfigItem, Func<ShopInGameStageLoadConfigItem, bool> selectAction, Action<ShopInGameStageLoadConfigItem> onConfirmedBuyItem)
        {
            var buffInGameDataConfig = await DataManager.Config.LoadShopInGameDataConfig(dataConfigItem.identity.shopInGameItemType);
            var (title, description) = await buffInGameDataConfig.GetDescription(dataConfigItem.identity.dataId);

            _description.text = description;
            _title.text = title;
            _price.text = (-dataConfigItem.cost.resourceNumber).ToString();

            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(() =>
            {
                var result = selectAction?.Invoke(dataConfigItem);
                if (result != null && result == true)
                {
                    onConfirmedBuyItem?.Invoke(dataConfigItem);
                }
            });

            LoadSpriteAsync(dataConfigItem.identity.shopInGameItemType, dataConfigItem.identity.dataId).Forget();
        }

        private async UniTaskVoid LoadSpriteAsync(ShopInGameItemType shopInGameItemType, int dataId)
        {
            _icon.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"shop_item_icon_{(int)shopInGameItemType}_{dataId}"), this.GetCancellationTokenOnDestroy());
        }
    }
}
