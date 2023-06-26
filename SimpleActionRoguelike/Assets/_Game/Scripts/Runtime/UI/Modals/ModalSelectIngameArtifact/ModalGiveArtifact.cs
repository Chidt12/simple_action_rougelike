using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Runtime.Message;

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

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _itemUIs = GetComponentsInChildren<GiveArtifactItemUI>();
            _buttons = _itemUIs.Select(x => x.GetComponent<Button>()).ToArray();
        }
#endif

        public override async UniTask Initialize(ModalGiveArtifactData data)
        {
            _currentSelectedIndex = -1;
            _isSelectedResetButton = false;
            _isSelected = false;

            GameManager.Instance.SetGameStateType(Definition.GameStateType.GameplayChoosingItem);

            for (int i = 0; i < data.Items.Length; i++)
            {
                var index = i;
                await _itemUIs[index].Init(data.EntityData, data.Items[index], (input) =>
                {
                    if (!_isSelected)
                    {
                        _isSelected = true;
                        data.OnSelectArtifact?.Invoke(input);
                        ScreenNavigator.Instance.PopModal(true).Forget();
                    }
                });
            }

            for (int i = 0; i < _itemUIs.Length; i++)
            {
                var index = i;
                var go = _itemUIs[index].gameObject;
                go.SetActive(index < data.Items.Length);
            }
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            base.OnKeyPress(message);
            if (!_isSelectedResetButton)
            {
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
            GameManager.Instance.ReturnPreviousGameStateType();
            return base.Cleanup();
        }
    }
}