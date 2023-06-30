using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Manager.Data;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class InventoryWeaponItemUI : MonoBehaviour, IInventoryItem
    {
        [SerializeField] private CustomButton _clickButton;
        [SerializeField] private Image _icon;

        private WeaponType _weaponType;
        private RarityType _rarityType;
        private bool _hasData;
        private Action<string> _loadInfoAction;

        public async UniTask LoadUI(WeaponType weaponType, RarityType rarityType, Action<string> loadInfoAction, CancellationToken token)
        {
            _hasData = true;
            _weaponType = weaponType;
            _rarityType = rarityType;

            _loadInfoAction = loadInfoAction;
            _icon.gameObject.SetActive(true);
            _icon.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"weapon_icon{(int)_weaponType}"), token);
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
            var buffInGameDataConfig = await DataManager.Config.LoadWeaponConfigItem(_weaponType);
            var description = await buffInGameDataConfig.GetDescription(_rarityType);
            _loadInfoAction?.Invoke(description);
        }
    }
}
