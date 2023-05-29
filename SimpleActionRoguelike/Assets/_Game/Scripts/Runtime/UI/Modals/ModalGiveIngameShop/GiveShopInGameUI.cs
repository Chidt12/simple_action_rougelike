using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Manager.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class GiveShopInGameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Button _selectButton;

        public async UniTask Init(ShopInGameStageLoadConfigItem dataConfigItem, Action<ShopInGameStageLoadConfigItem> selectAction)
        {
            var buffInGameDataConfig = await DataManager.Config.LoadShopInGameDataConfig(dataConfigItem.identity.shopInGameItemType);
            var description = await buffInGameDataConfig.GetDescription(dataConfigItem.identity.dataId);

            _description.text = description;
            _title.text = dataConfigItem.identity.shopInGameItemType.ToString();

            _selectButton.onClick.RemoveAllListeners();
            _selectButton.onClick.AddListener(() =>
            {
                selectAction?.Invoke(dataConfigItem);
            });
        }
    }
}