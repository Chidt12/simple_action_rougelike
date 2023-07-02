using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Manager;
using Runtime.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ModalBuyIngameShopData
    {
        public List<ShopInGameStageLoadConfigItem> items;
        public Func<ShopInGameStageLoadConfigItem, bool> onSelectShopInGameItem;

        public ModalBuyIngameShopData(List<ShopInGameStageLoadConfigItem> items, Func<ShopInGameStageLoadConfigItem, bool> onSelectShopInGameItem)
        {
            this.onSelectShopInGameItem = onSelectShopInGameItem;
            this.items = items;
        }
    }

    public class ModalBuyIngameShop : Modal<ModalBuyIngameShopData>
    {
        [SerializeField] private ShopInGameItemUI[] _itemUIs;
        [SerializeField] private Button[] _buttons;
        [SerializeField] private Button _resetButton;

        private int _currentSelectedIndex;
        private bool _isSelectedResetButton;
        private ModalBuyIngameShopData _data;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _itemUIs = GetComponentsInChildren<ShopInGameItemUI>();
        }
#endif
        public async override UniTask Initialize(ModalBuyIngameShopData data)
        {
            _data = data;
            GameManager.Instance.SetGameStateType(Definition.GameStateType.GameplayBuyingItem, true);
            await UpdateUI();
        }

        private async UniTask UpdateUI()
        {
            if(_currentSelectedIndex != -1)
            {
                if (_isSelectedResetButton)
                    ExitAButton(_resetButton);
                else
                    ExitAButton(_buttons[_currentSelectedIndex]);
            }

            _currentSelectedIndex = -1;
            _isSelectedResetButton = false;

            for (int i = 0; i < _data.items.Count; i++)
            {
                await _itemUIs[i].Init(_data.items[i], _data.onSelectShopInGameItem, OnConfirmedBuy);
            }

            for (int i = 0; i < _itemUIs.Length; i++)
            {
                _itemUIs[i].gameObject.SetActive(i < _data.items.Count);
            }
        }

        private void OnConfirmedBuy(ShopInGameStageLoadConfigItem obj)
        {
            _data.items.Remove(obj);
            if (_data.items.Count <= 0)
                ScreenNavigator.Instance.PopModal(true).Forget();
            else
                UpdateUI().Forget();
        }

        public override UniTask Cleanup()
        {
            GameManager.Instance.ReturnPreviousGameState();
            return base.Cleanup();
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            base.OnKeyPress(message);
            if (!_isSelectedResetButton)
            {
                if (message.KeyPressType == KeyPressType.Right)
                {
                    if (_currentSelectedIndex < _data.items.Count - 1)
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

    }
}