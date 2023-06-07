using Cysharp.Threading.Tasks;
using Runtime.Message;
using System;
using UnityEngine;

namespace Runtime.UI
{
    public class ScreenHome : BaseScreen
    {
        [SerializeField] private CustomButton[] _selectButtons;

        private int _currentButtonIndex;

        public override UniTask Initialize(Memory<object> args)
        {
            Select(_selectButtons[0]);
            _currentButtonIndex = 0;
            return base.Initialize(args);
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            base.OnKeyPress(message);
            if (message.KeyPressType == KeyPressType.Up)
            {
                if(_currentButtonIndex > 0)
                {
                    DeSelect(_selectButtons[_currentButtonIndex]);
                    _currentButtonIndex--;
                    Select(_selectButtons[_currentButtonIndex]);
                }
            }
            else if (message.KeyPressType == KeyPressType.Down)
            {
                if (_currentButtonIndex < _selectButtons.Length - 1)
                {
                    DeSelect(_selectButtons[_currentButtonIndex]);
                    _currentButtonIndex++;
                    Select(_selectButtons[_currentButtonIndex]);
                }
            }
        }
    }
}