using Cysharp.Threading.Tasks;
using DanielLochner.Assets.SimpleScrollSnap;
using Runtime.Definition;
using Runtime.Localization;
using Runtime.Manager;
using Runtime.Manager.Data;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ScreenLobby : BaseScreen
    {
        [SerializeField] private SimpleScrollSnap _simpleScrollSnap;
        [SerializeField] private StatItemUI[] _statItems;
        [SerializeField] private WeaponSelectUI[] _weaponSelectUIs;
        [SerializeField] private WeaponVisualUI[] _weaponVisualUIs;
        [SerializeField] private Button _previousWeaponButton;
        [SerializeField] private Button _nextWeaponButton;
        [SerializeField] private Button _startGameButton;

        private CancellationTokenSource _updateUICancellationTokenSource;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _weaponSelectUIs = GetComponentsInChildren<WeaponSelectUI>(true);
            _weaponVisualUIs = GetComponentsInChildren<WeaponVisualUI>(true);
            _statItems = GetComponentsInChildren<StatItemUI>(true);
        }
#endif

        public async override UniTask Initialize(Memory<object> args)
        {
            await base.Initialize(args);
            GameManager.Instance.SetGameStateType(GameStateType.Lobby);

            _startGameButton.onClick.RemoveAllListeners();
            _startGameButton.onClick.AddListener(OnStartGame);

            foreach (var statItem in _statItems)
                statItem.gameObject.SetActive(false);

            var playerBasicLocalData = DataManager.Local.playerBasicLocalData;
            var currentSelectedWeapon = playerBasicLocalData.selectedWeapon;
            UpdateUIAsync(currentSelectedWeapon).Forget();

        }

        public override UniTask Cleanup()
        {
            _updateUICancellationTokenSource?.Cancel();
            GameManager.Instance.ReturnPreviousGameStateType();
            return base.Cleanup();
        }

        public override void DidPushEnter(Memory<object> args)
        {
            base.DidPushEnter(args);
            var playerBasicLocalData = DataManager.Local.playerBasicLocalData;
            var currentSelectedWeapon = playerBasicLocalData.selectedWeapon;
            var goToIndex = FindIndexWeapon(currentSelectedWeapon);
            _simpleScrollSnap.OnPanelSelected.RemoveAllListeners();
            _simpleScrollSnap.GoToPanel(goToIndex);
            _simpleScrollSnap.OnPanelSelected.AddListener(OnSelectedWeapon);
        }

        private void OnSelectedWeapon(int selectedIndex)
        {
            var selectedWeaponVisualUI = _simpleScrollSnap.Panels[_simpleScrollSnap.CenteredPanel].GetComponent<WeaponSelectUI>();
            DataManager.Local.playerBasicLocalData.selectedWeapon = selectedWeaponVisualUI.WeaponType;
            UpdateUIAsync(selectedWeaponVisualUI.WeaponType).Forget();
        }

        private async UniTaskVoid UpdateUIAsync(WeaponType weaponType)
        {
            foreach (var weaponVisualUI in _weaponVisualUIs)
            {
                if (weaponVisualUI.weaponType == weaponType)
                    weaponVisualUI.gameObject.SetActive(true);
                else
                    weaponVisualUI.gameObject.SetActive(false);
            }

            foreach (var statItem in _statItems)
                statItem.gameObject.SetActive(false);

            var weaponConfigItem = await DataManager.Config.LoadWeaponDataConfigItem(weaponType);

            foreach (var item in _statItems)
            {
                var statBonus = weaponConfigItem.stats.FirstOrDefault(x => x.statType == item.StatType);
                var stringValue = $"{(statBonus.statType.IsPercentValue() ? statBonus.value * 100 + "%" : statBonus.value)}";
                item.SetValue(stringValue);
                item.gameObject.SetActive(true);
            }
        }

        public int FindIndexWeapon(WeaponType weaponType)
        {
            for (int i = 0; i < _weaponSelectUIs.Length; i++)
            {
                if (_weaponSelectUIs[i].WeaponType == weaponType)
                    return i;
            }

            return 0;
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            base.OnKeyPress(message);
            if(message.KeyPressType == KeyPressType.Right)
            {
                Submit(_previousWeaponButton);
            }
            else if (message.KeyPressType == KeyPressType.Left)
            {
                Submit(_nextWeaponButton);
            }
            else if (message.KeyPressType == KeyPressType.Confirm)
            {
                Submit(_startGameButton);
            }
        }

        private void OnStartGame()
        {
            DataManager.Local.Set(DataManager.Local.playerBasicLocalData);
            GameManager.Instance.StartLoadingGameplayAsync().Forget();
        }
    }
}