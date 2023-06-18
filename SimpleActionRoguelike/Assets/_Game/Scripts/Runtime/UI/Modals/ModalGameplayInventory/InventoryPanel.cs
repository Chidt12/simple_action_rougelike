using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using Runtime.Manager.Gameplay;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private StatItemUI[] _stats;
        [SerializeField] private InventoryArtifactItemUI[] _artifacts;
        [SerializeField] private InventoryShopItemUI[] _shopItems;
        [SerializeField] private TextMeshProUGUI _infoText;

        public UniTask LoadUI()
        {
            LoadStats().Forget();
            LoadShopItems().Forget();
            LoadArtifactItems().Forget();

            return UniTask.CompletedTask;
        }

        private async UniTask LoadStats()
        {
            var statData = EntitiesManager.Instance.HeroData as IEntityStatData;
            foreach (var item in _stats)
            {
                var statValue = statData.GetTotalStatValue(item.StatType);
                var statName = await LocalizeManager.GetLocalizeAsync(LocalizeTable.GENERAL, LocalizeKeys.GetStatName(item.StatType));
                var stringValue = $"{statName}: {(item.StatType.IsPercentValue() ? statValue * 100 + "%" : statValue)}";
                item.SetValue(stringValue);
                item.gameObject.SetActive(true);
            }
        }

        private UniTask LoadShopItems()
        {
            var allItems = GameplayManager.Instance.CurrentShopInGameItems;
            if(allItems.Count > 0)
            {
                for (int i = 0; i < allItems.Count; i++)
                {
                    var item = allItems[i];
                    _shopItems[i].LoadUI(item.ShopInGameItemType, item.DataId, OnChangeInfo).Forget();
                }
            }

            return UniTask.CompletedTask;
        }

        private async UniTask LoadArtifactItems()
        {
            var allItems = GameplayManager.Instance.CurrentBuffInGameItems;
            foreach (var item in allItems)
            {

            }
        }

        private void OnChangeInfo(string info)
        {
            _infoText.text = info;
        }
    }
}