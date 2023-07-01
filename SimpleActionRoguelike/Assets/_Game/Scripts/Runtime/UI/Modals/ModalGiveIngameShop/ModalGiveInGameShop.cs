using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Manager;
using Runtime.Message;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Runtime.UI
{
    public class ModalGiveInGameShopData
    {
        public readonly ShopInGameStageLoadConfigItem[] Items;
        public readonly Action<ShopInGameStageLoadConfigItem> OnSelectShopInGameItem;

        public ModalGiveInGameShopData(ShopInGameStageLoadConfigItem[] items, Action<ShopInGameStageLoadConfigItem> onSelectShopInGameItem)
        {
            OnSelectShopInGameItem = onSelectShopInGameItem;
            Items = items;
        }
    }

    public class ModalGiveInGameShop : Modal<ModalGiveInGameShopData>
    {
        [SerializeField] private GiveShopInGameUI[] _itemUIs;
        [SerializeField] private Button[] _buttons;
        [SerializeField] private Button _resetButton;
 
        private ModalGiveInGameShopData _data;
        private int _currentSelectedIndex;
        private bool _isSelected;
        private bool _isSelectedResetButton;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _itemUIs = GetComponentsInChildren<GiveShopInGameUI>();
            _buttons = _itemUIs.Select(x => x.GetComponent<Button>()).ToArray();
        }
#endif

        public override async UniTask Initialize(ModalGiveInGameShopData data)
        {
            _currentSelectedIndex = -1;
            _isSelectedResetButton = false;
            _isSelected = false;
            _data = data;
            GameManager.Instance.SetGameStateType(Definition.GameStateType.GameplayChoosingItem, true);

            for (int i = 0; i < _itemUIs.Length; i++)
            {
                _itemUIs[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < _data.Items.Length; i++)
            {
                _itemUIs[i].gameObject.SetActive(true);
                await _itemUIs[i].Init(_data.Items[i], (input) =>
                {
                    if (!_isSelected)
                    {
                        _isSelected = true;
                        _data.OnSelectShopInGameItem?.Invoke(input);
                        ScreenNavigator.Instance.PopModal(true).Forget();
                    }
                });
            }
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