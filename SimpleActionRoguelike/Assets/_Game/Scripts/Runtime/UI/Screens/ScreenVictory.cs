using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Manager;
using Runtime.Message;
using System;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class ScreenVictory : BaseScreen
    {
        #region Members

        [SerializeField] private CustomButton[] _selectButtons;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private float _delayInteract = 1f;

        private float _startTime;

        #endregion Members

        #region Class Methods

        public override UniTask Initialize(Memory<object> args)
        {
            GameManager.Instance.SetGameStateType(GameStateType.WinGameplay);
            for (int i = 0; i < _selectButtons.Length; i++)
            {
                _selectButtons[i].Index = i;
                _selectButtons[i].CustomPointEnterAction = OnEnterAnItem;
            }

            _selectButtons[0].onClick.RemoveAllListeners();
            _selectButtons[0].onClick.AddListener(OnReplay);

            _selectButtons[1].onClick.RemoveAllListeners();
            _selectButtons[1].onClick.AddListener(OnBackHome);

            EnterAButton(_selectButtons[0]);
            currentSelectedIndex = 0;
            _startTime = Time.realtimeSinceStartup;
            return base.Initialize(args);
        }

        public override UniTask Cleanup()
        {
            GameManager.Instance.ReturnPreviousGameStateType();
            return base.Cleanup();
        }

        private void OnEnterAnItem(int index)
        {
            _selectButtons[currentSelectedIndex].ToggleSelect(false);
            currentSelectedIndex = index;
            _selectButtons[currentSelectedIndex].ToggleSelect(true);
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            base.OnKeyPress(message);

            if (Time.realtimeSinceStartup - _startTime <= _delayInteract)
                return;

            if (message.KeyPressType == KeyPressType.Confirm)
            {
                Submit(_selectButtons[currentSelectedIndex]);
            }
            else if (message.KeyPressType == KeyPressType.Left)
            {
                if (currentSelectedIndex > 0)
                {
                    EnterAButton(_selectButtons[currentSelectedIndex - 1]);
                }
            }
            else if (message.KeyPressType == KeyPressType.Right)
            {
                if (currentSelectedIndex < _selectButtons.Length - 1)
                {
                    EnterAButton(_selectButtons[currentSelectedIndex + 1]);
                }
            }
        }

        private void OnBackHome()
        {
            GameManager.Instance.BackHomeAsync().Forget();
        }

        private void OnReplay()
        {
            GameManager.Instance.Replay().Forget();
        }

        #endregion Class Methods
    }

}