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

        private ModalGiveInGameShopData _data;
        private int _currentSelectedIndex;
        private bool _isSelected;

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
            _isSelected = false;
            _data = data;
            GameManager.Instance.SetGameStateType(Definition.GameStateType.GameplayChoosingItem);

            for (int i = 0; i < _itemUIs.Length; i++)
            {
                _itemUIs[i].gameObject.SetActive(false);
            }
        }

        public override void DidPushEnter(Memory<object> args)
        {
            base.DidPushEnter(args);
            LoadUiAsync().Forget();
        }

        private async UniTask LoadUiAsync()
        {
            for (int i = 0; i < _data.Items.Length; i++)
            {
                await _itemUIs[i].Init(_data.Items[i], (input) =>
                {
                    if (!_isSelected)
                    {
                        _isSelected = true;
                        _data.OnSelectShopInGameItem?.Invoke(input);
                        ScreenNavigator.Instance.PopModal(true).Forget();
                    }
                });
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            base.OnKeyPress(message);
            if (message.KeyPressType == KeyPressType.Right) 
            {
                if (_currentSelectedIndex < _buttons.Length - 1)
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
            }
        }

        public override UniTask Cleanup()
        {
            GameManager.Instance.ReturnPreviousGameStateType();
            return base.Cleanup();
        }
    }
}