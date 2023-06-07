using Cysharp.Threading.Tasks;
using Runtime.Message;
using System;
using UnityEngine;

namespace Runtime.UI
{
    public class ModalQuitGame : BaseModal
    {
        [SerializeField] private CustomButton _yesButton;
        [SerializeField] private CustomButton _noButton;

        public override UniTask Initialize(Memory<object> args)
        {
            _yesButton.Index = 0;
            _noButton.Index = 1;
            _yesButton.CustomPointEnterAction = OnEnterAnItem;
            _noButton.CustomPointEnterAction = OnEnterAnItem;

            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();

            _yesButton.onClick.AddListener(() => Application.Quit());
            _noButton.onClick.AddListener(() => ScreenNavigator.Instance.PopModal(true).Forget());

            EnterAButton(_yesButton);
            currentSelectedIndex = 0;
            return base.Initialize(args);
        }

        private void OnEnterAnItem(int index)
        {
            currentSelectedIndex = index;
            if (index == 0)
            {
                _yesButton.ToggleSelect(true);
                _noButton.ToggleSelect(false);
            }
            else
            {
                _yesButton.ToggleSelect(false);
                _noButton.ToggleSelect(true);
            }
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            base.OnKeyPress(message);
            if (message.KeyPressType == KeyPressType.Right)
            {
                EnterAButton(_noButton);
            }
            else if (message.KeyPressType == KeyPressType.Left)
            {
                EnterAButton(_yesButton);
            }
            else if (message.KeyPressType == KeyPressType.Confirm)
            {
                if (currentSelectedIndex == 0)
                    Submit(_yesButton);
                else
                    Submit(_noButton);
            }
        }
    }
}