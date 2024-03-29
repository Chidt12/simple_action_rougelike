using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Runtime.Message;
using Runtime.Definition;
using Runtime.Manager.Data;
using Runtime.Manager.Gameplay;

namespace Runtime.UI
{
    public class ModalGiveArtifactData
    {
        public readonly IEntityData EntityData;
        public readonly ArtifactIdentity[] Items;
        public readonly Action<ArtifactIdentity> OnSelectArtifact;

        public ModalGiveArtifactData(IEntityData entityData, ArtifactIdentity[] items, Action<ArtifactIdentity> onSelectItemBuff)
        {
            EntityData = entityData;
            Items = items;
            OnSelectArtifact = onSelectItemBuff;
        }
    }

    public class ModalGiveArtifact : Modal<ModalGiveArtifactData>
    {
        [SerializeField] private GiveArtifactItemUI[] _itemUIs;
        [SerializeField] private Button[] _buttons;
        [SerializeField] private Button _resetButton;

        private int _currentSelectedIndex;
        private bool _isSelected;
        private bool _isSelectedResetButton;
        private ModalGiveArtifactData _data;
        private ArtifactIdentity[] _items;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _itemUIs = GetComponentsInChildren<GiveArtifactItemUI>();
            _buttons = _itemUIs.Select(x => x.GetComponent<Button>()).ToArray();
        }
#endif

        public override async UniTask Initialize(ModalGiveArtifactData data)
        {
            _data = data;
            _items = data.Items;
            _currentSelectedIndex = -1;
            _isSelectedResetButton = false;
            _isSelected = false;

            GameManager.Instance.SetGameStateType(Definition.GameStateType.GameplayChoosingItem, true);

            await UpdateUI();

            _resetButton.onClick.RemoveAllListeners();
            _resetButton.onClick.AddListener(OnReset);
        }

        private async UniTask UpdateUI()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                var index = i;
                await _itemUIs[index].Init(_data.EntityData, _items[index], (input) =>
                {
                    if (!_isSelected)
                    {
                        _isSelected = true;
                        _data.OnSelectArtifact?.Invoke(input);
                        ScreenNavigator.Instance.PopModal(true).Forget();
                    }
                });
            }

            for (int i = 0; i < _itemUIs.Length; i++)
            {
                var index = i;
                var go = _itemUIs[index].gameObject;
                go.SetActive(index < _items.Length);
            }
        }

        private void OnReset()
        {
            var value = DataManager.Transient.GetGameMoneyType(InGameMoneyType.Gold);
            if (value < GameplayManager.RESET_COST)
            {
                ToastController.Instance.Show("Not Enough Resource!");
                return;
            }

            DataManager.Transient.RemoveMoney(InGameMoneyType.Gold, GameplayManager.RESET_COST);
            OnResetAsync().Forget();
        }

        private async UniTaskVoid OnResetAsync()
        {
            var items = await DataManager.Config.LoadCurrentSuitableArtifactItems(GameplayManager.Instance.CurrentBuffInGameItems, GameplayManager.NUMBER_OF_SELECT_ARTIFACT);
            _items = items.Select(x => x.identity).ToArray();
            await UpdateUI();
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            base.OnKeyPress(message);
            if (!_isSelectedResetButton)
            {
                if (message.KeyPressType == KeyPressType.Right)
                {
                    if (_currentSelectedIndex < _data.Items.Length - 1)
                    {
                        if (_currentSelectedIndex != -1)
                            ExitAButton(_buttons[_currentSelectedIndex]);
                        _currentSelectedIndex++;
                        EnterAButton(_buttons[_currentSelectedIndex]);
                    }
                }
                else if (message.KeyPressType == KeyPressType.Left)
                {
                    if (_currentSelectedIndex > 0)
                    {
                        ExitAButton(_buttons[_currentSelectedIndex]);
                        _currentSelectedIndex--;
                        EnterAButton(_buttons[_currentSelectedIndex]);
                    }
                    else
                    {
                        _currentSelectedIndex = 0;
                        EnterAButton(_buttons[_currentSelectedIndex]);
                    }
                }
            }

            if (message.KeyPressType == KeyPressType.Down)
            {
                if (_currentSelectedIndex != -1)
                    ExitAButton(_buttons[_currentSelectedIndex]);
                EnterAButton(_resetButton);
                _isSelectedResetButton = true;
            }
            else if (message.KeyPressType == KeyPressType.Up)
            {
                if (_isSelectedResetButton)
                {
                    ExitAButton(_resetButton);
                    _isSelectedResetButton = false;
                }

                if (_currentSelectedIndex == -1)
                {
                    _currentSelectedIndex = 0;
                }

                EnterAButton(_buttons[_currentSelectedIndex]);
            }
            else if (message.KeyPressType == KeyPressType.Confirm)
            {
                if (_isSelectedResetButton)
                {
                    Submit(_resetButton);
                }
                else
                {
                    if (_currentSelectedIndex != -1)
                    {
                        Submit(_buttons[_currentSelectedIndex]);
                    }
                    else
                    {

                    }
                }
            }
        }

        public override UniTask Cleanup()
        {
            GameManager.Instance.ReturnPreviousGameState();
            return base.Cleanup();
        }
    }
}