using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Manager.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Runtime.Definition;

namespace Runtime.UI
{
    public class GiveShopInGameUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Button _selectButton;

        public async UniTask Init(ShopInGameStageLoadConfigItem dataConfigItem, Action<ShopInGameStageLoadConfigItem> selectAction)
        {
            var buffInGameDataConfig = await DataManager.Config.LoadShopInGameDataConfig(dataConfigItem.identity.shopInGameItemType);
            var (title, description) = await buffInGameDataConfig.GetDescription(dataConfigItem.identity.dataId);

            _description.text = description;
            _title.text = title;

            _selectButton.onClick.RemoveAllListeners();
            _selectButton.onClick.AddListener(() =>
            {
                selectAction?.Invoke(dataConfigItem);
            });

            LoadSpriteAsync(dataConfigItem.identity.shopInGameItemType, dataConfigItem.identity.dataId).Forget();
        }

        private async UniTaskVoid LoadSpriteAsync(ShopInGameItemType shopInGameItemType, int dataId)
        {
            _icon.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"shop_item_icon_{(int)shopInGameItemType}_{dataId}"), this.GetCancellationTokenOnDestroy());
        }
    }
}