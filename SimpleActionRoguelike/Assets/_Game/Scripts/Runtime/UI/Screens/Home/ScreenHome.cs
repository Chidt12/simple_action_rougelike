using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Definition;
using Runtime.Localization;
using Runtime.Manager;
using Runtime.Message;
using System;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Views;

namespace Runtime.UI
{
    public class ScreenHome : BaseScreen
    {
        [SerializeField] private CustomButton[] _selectButtons;

        public override UniTask Initialize(Memory<object> args)
        {
            for (int i = 0; i < _selectButtons.Length; i++)
            {
                _selectButtons[i].Index = i;
                _selectButtons[i].CustomPointEnterAction = OnEnterAnItem;
            }

            _selectButtons[0].onClick.RemoveAllListeners();
            _selectButtons[0].onClick.AddListener(OnClickNewGame);

            _selectButtons[1].onClick.RemoveAllListeners();
            _selectButtons[1].onClick.AddListener(() =>
            {
                var options = new WindowOptions(ModalIds.GAME_SETTINGS);
                ScreenNavigator.Instance.LoadModal(options).Forget();
            });

            _selectButtons[2].onClick.RemoveAllListeners();
            _selectButtons[2].onClick.AddListener(OnClickQuit);

            EnterAButton(_selectButtons[0]);
            currentSelectedIndex = 0;
            return base.Initialize(args);
        }

        private void OnClickNewGame()
        {
            GameManager.Instance.GoToLobbyScreen().Forget();
        }

        private void OnClickQuit()
        {
            var content = LocalizeManager.GetLocalize(LocalizeTable.UI, LocalizeKeys.POPUP_CONFIRM_QUIT_GAME);
            var windowOptions = new WindowOptions(ModalIds.CONFIRM_ACTION);
            ScreenNavigator.Instance.LoadModal(windowOptions, new ModalConfirmActionData(content, () => Application.Quit())).Forget();
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
            if (message.KeyPressType == KeyPressType.Up)
            {
                if (currentSelectedIndex > 0)
                {
                    EnterAButton(_selectButtons[currentSelectedIndex - 1]);
                }
            }
            else if (message.KeyPressType == KeyPressType.Down)
            {
                if (currentSelectedIndex < _selectButtons.Length - 1)
                {
                    EnterAButton(_selectButtons[currentSelectedIndex + 1]);
                }
            }
            else if (message.KeyPressType == KeyPressType.Confirm)
            {
                Submit(_selectButtons[currentSelectedIndex]);
            }
        }
    }
}