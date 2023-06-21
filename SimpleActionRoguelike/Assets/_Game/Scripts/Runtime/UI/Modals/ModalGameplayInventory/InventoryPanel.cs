using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public interface IInventoryItem
    {
        void ToggleSelect(bool value);
    }

    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private StatItemUI[] _stats;
        [SerializeField] private InventoryArtifactItemUI[] _artifacts;
        [SerializeField] private InventoryShopItemUI[] _shopItems;
        [SerializeField] private TextMeshProUGUI _infoText;
        [SerializeField] private int _numberShopItemInHorizontal = 5;
        [SerializeField] private int _numberShopItemInVertical = 4;

        private IInventoryItem _inventoryItem;

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
                    artifact.LoadUI(buffItem.artifactType, buffItem.level, OnChangeInfo).Forget();
                }
                else
                {
                    artifact.ClearUI(OnChangeInfo);
                }
            }

            UpdateToggle(_artifacts[0]);
            return UniTask.CompletedTask;
        }

        private void UpdateToggle(IInventoryItem inventoryItem)
        {
            if(_inventoryItem != null)
            {
                _inventoryItem.ToggleSelect(false);
            }

            _inventoryItem = inventoryItem;
            _inventoryItem.ToggleSelect(true);
        }

        private void OnChangeInfo(string info)
        {
            _infoText.text = info;
        }

        public void OnKeyPress(InputKeyPressMessage message)
        {
            if (!IsSelected)
                return;

            if(message.KeyPressType == KeyPressType.Right || message.KeyPressType == KeyPressType.Left || message.KeyPressType == KeyPressType.Up || message.KeyPressType == KeyPressType.Down)
            {
                if (_inventoryItem != null)
                {
                    if (_inventoryItem is InventoryArtifactItemUI)
                    {
                        var index = Array.IndexOf(_artifacts, _inventoryItem);
                        if(message.KeyPressType == KeyPressType.Right && index < _artifacts.Length - 1)
                        {
                            var nextIndex = index + 1;
                            UpdateToggle(_artifacts[nextIndex]);
                        }
                        else if (message.KeyPressType == KeyPressType.Left && index > 0)
                        {
                            var nextIndex = index - 1;
                            UpdateToggle(_artifacts[nextIndex]);
                        }
                        else if (message.KeyPressType == KeyPressType.Down)
                        {
                            UpdateToggle(_shopItems[index]);
                        }
                    }
                    else if (_inventoryItem is InventoryShopItemUI)
                    {
                        var index = Array.IndexOf(_shopItems, _inventoryItem);
                        if (message.KeyPressType == KeyPressType.Right && index % _numberShopItemInHorizontal < _numberShopItemInHorizontal - 1)
                        {
                            var nextIndex = index + 1;
                            UpdateToggle(_shopItems[nextIndex]);
                        }
                        else if (message.KeyPressType == KeyPressType.Left && index % _numberShopItemInHorizontal > 0)
                        {
                            var nextIndex = index - 1;
                            UpdateToggle(_shopItems[nextIndex]);
                        }
                        else if (message.KeyPressType == KeyPressType.Down && index < _shopItems.Length - 1 - _numberShopItemInHorizontal)
                        {
                            var nextIndex = index + _numberShopItemInHorizontal;
                            UpdateToggle(_shopItems[nextIndex]);
                        }
                        else if (message.KeyPressType == KeyPressType.Up)
                        {
                            if(index > _numberShopItemInHorizontal - 1)
                            {
                                var nextIndex = index - _numberShopItemInHorizontal;
                                UpdateToggle(_shopItems[nextIndex]);
                            }
                            else
                            {
                                
                                UpdateToggle(_artifacts[index]);
                            }
                        }
                    }
                }
            }
        }
    }
}