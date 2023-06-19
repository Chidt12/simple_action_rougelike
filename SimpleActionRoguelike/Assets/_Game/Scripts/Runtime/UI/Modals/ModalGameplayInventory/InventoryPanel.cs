using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public interface InventoryItem
    {
        void ToggleSelect(bool value);
    }

    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private StatItemUI[] _stats;
        [SerializeField] private InventoryArtifactItemUI[] _artifacts;
        [SerializeField] private InventoryShopItemUI[] _shopItems;
        [SerializeField] private TextMeshProUGUI _infoText;
        [SerializeField] private int numberShopItemInHorizontal = 5;
        [SerializeField] private int numberShopItemInVertical = 4;

        public bool IsSelected { get; set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _stats = GetComponentsInChildren<StatItemUI>();
            _artifacts = GetComponentsInChildren<InventoryArtifactItemUI>();
            _shopItems = GetComponentsInChildren<InventoryShopItemUI>();
        }
#endif

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

            for (int i = 0; i < _shopItems.Length; i++)
            {
                var shopItem = _shopItems[i];
                if(i < allItems.Count)
                {
                    var item = allItems[i];
                    shopItem.LoadUI(item.ShopInGameItemType, item.DataId, OnChangeInfo).Forget();
                }
                else
                {
                    shopItem.ClearUI(OnChangeInfo);
                }
            }

            return UniTask.CompletedTask;
        }

        private UniTask LoadArtifactItems()
        {
            var allItems = GameplayManager.Instance.CurrentBuffInGameItems;
            for (int i = 0; i < _artifacts.Length; i++)
            {
                var artifact = _artifacts[i];
                if (i < allItems.Count)
                {
                    var buffItem = allItems[i];
                    artifact.LoadUI(buffItem.buffInGameType, buffItem.level, OnChangeInfo).Forget();
                }
                else
                {
                    artifact.ClearUI(OnChangeInfo);
                }
            }

            return UniTask.CompletedTask;
        }

        private void OnChangeInfo(string info)
        {
            _infoText.text = info;
        }

        public void OnKeyPress(InputKeyPressMessage message)
        {

        }
    }
}