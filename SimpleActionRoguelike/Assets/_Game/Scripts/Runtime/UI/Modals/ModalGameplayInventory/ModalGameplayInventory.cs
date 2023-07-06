using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager;
using Runtime.Manager.Data;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public interface IInventoryItem 
    {
        public void ToggleSelect(bool value);
    }

    public class ModalGameplayInventory : BaseModal
    {
        [SerializeField] private StatItemUI[] _stats;
        [SerializeField] private InventoryWeaponItemUI _weaponItem;
        [SerializeField] private InventoryArtifactItemUI[] _artifacts;
        [SerializeField] private InventoryShopItemUI[] _shopItems;
        [SerializeField] private TextMeshProUGUI _infoText;
        [SerializeField] private int _numberShopItemInHorizontal = 5;
        [SerializeField] private int _numberShopItemInVertical = 4;

        private CancellationTokenSource _cancellationTokenSource;
        private IInventoryItem _inventoryItem;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _weaponItem = GetComponentInChildren<InventoryWeaponItemUI>();
            _stats = GetComponentsInChildren<StatItemUI>();
            _artifacts = GetComponentsInChildren<InventoryArtifactItemUI>();
            _shopItems = GetComponentsInChildren<InventoryShopItemUI>();
        }
#endif

        public async override UniTask Initialize(Memory<object> args)
        {
            await base.Initialize(args);
            GameManager.Instance.SetGameStateType(GameStateType.GameplayPausing, true);
            _cancellationTokenSource = new();
            await LoadUI();
        }

        public override UniTask Cleanup()
        {
            GameManager.Instance.ReturnPreviousGameState();
            _cancellationTokenSource?.Cancel();
            return base.Cleanup();
        }

        public async UniTask LoadUI()
        {
            LoadStats();
            await LoadWeaponItem();
            await LoadShopItems();
            await LoadArtifactItems();
        }

        private void LoadStats()
        {
            var statData = EntitiesManager.Instance.HeroData as IEntityStatData;
            foreach (var item in _stats)
            {
                var statValue = statData.GetTotalStatValue(item.StatType);
                var stringValue = $"{(item.StatType.IsPercentValue() ? statValue * 100 + "%" : statValue)}";
                item.SetValue(stringValue);
                item.gameObject.SetActive(true);
            }
        }

        private async UniTask LoadWeaponItem()
        {
            var weapon = EntitiesManager.Instance.HeroData as IEntityWeaponData;
            var weaponModel = weapon.WeaponModel;

            var currentUpgradeArtifact = GameplayManager.Instance.CurrentBuffInGameItems.FirstOrDefault(x => x.artifactType == ArtifactType.UpgradeWeapon);
            if (currentUpgradeArtifact.artifactType == ArtifactType.UpgradeWeapon)
            {
                var artifactUpgradeWeapon = await DataManager.Config.LoadArtifactDataConfigItem(ArtifactType.UpgradeWeapon, currentUpgradeArtifact.level) as UpgradeWeaponArtifactDataConfigItem;
                await _weaponItem.LoadUI(weaponModel.WeaponType, artifactUpgradeWeapon.rarityType, OnChangeInfo, _cancellationTokenSource.Token);
            }
            else
            {
                await _weaponItem.LoadUI(weaponModel.WeaponType, RarityType.Common, OnChangeInfo, _cancellationTokenSource.Token);
            }
            UpdateToggle(_weaponItem);
        }

        private UniTask LoadShopItems()
        {
            var allItems = GameplayManager.Instance.CurrentShopInGameItems;

            for (int i = 0; i < _shopItems.Length; i++)
            {
                var shopItem = _shopItems[i];
                if (i < allItems.Count)
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
            var allItems = GameplayManager.Instance.CurrentBuffInGameItems.Where(x => x.artifactType != ArtifactType.UpgradeWeapon).ToList();
            for (int i = 0; i < _artifacts.Length; i++)
            {
                var artifact = _artifacts[i];
                if (i < allItems.Count)
                {
                    var buffItem = allItems[i];
                    artifact.LoadUI(buffItem.artifactType, buffItem.level, OnChangeInfo, _cancellationTokenSource.Token).Forget();
                }
                else
                {
                    artifact.ClearUI(OnChangeInfo);
                }
            }

            return UniTask.CompletedTask;
        }

        private void UpdateToggle(IInventoryItem inventoryItem)
        {
            if (_inventoryItem != null)
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

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            if (message.KeyPressType == KeyPressType.Right || message.KeyPressType == KeyPressType.Left || message.KeyPressType == KeyPressType.Up || message.KeyPressType == KeyPressType.Down)
            {
                if (_inventoryItem != null)
                {
                    if (_inventoryItem is InventoryArtifactItemUI)
                    {
                        var index = Array.IndexOf(_artifacts, _inventoryItem);
                        if (message.KeyPressType == KeyPressType.Right && index < _artifacts.Length - 1)
                        {
                            var nextIndex = index + 1;
                            UpdateToggle(_artifacts[nextIndex]);
                        }
                        else if (message.KeyPressType == KeyPressType.Left)
                        {
                            if(index > 0)
                            {
                                var nextIndex = index - 1;
                                UpdateToggle(_artifacts[nextIndex]);
                            }
                            else
                            {
                                UpdateToggle(_weaponItem);
                            }
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
                        else if (message.KeyPressType == KeyPressType.Down && index <= _shopItems.Length - 1 - _numberShopItemInHorizontal)
                        {
                            var nextIndex = index + _numberShopItemInHorizontal;
                            UpdateToggle(_shopItems[nextIndex]);
                        }
                        else if (message.KeyPressType == KeyPressType.Up)
                        {
                            if (index > _numberShopItemInHorizontal - 1)
                            {
                                var nextIndex = index - _numberShopItemInHorizontal;
                                UpdateToggle(_shopItems[nextIndex]);
                            }
                            else
                            {
                                var artifactIndex = index - 1;
                                if(artifactIndex < 0)
                                {
                                    UpdateToggle(_weaponItem);
                                }
                                else
                                {
                                    UpdateToggle(_artifacts[artifactIndex]);
                                }
                            }
                        }
                    }
                    else if (_inventoryItem is InventoryWeaponItemUI)
                    {
                        if(message.KeyPressType == KeyPressType.Right)
                        {
                            UpdateToggle(_artifacts[0]);
                        }
                        else if (message.KeyPressType == KeyPressType.Down)
                        {
                            UpdateToggle(_shopItems[0]);
                        }
                    }
                }
            }
        }
    }
}