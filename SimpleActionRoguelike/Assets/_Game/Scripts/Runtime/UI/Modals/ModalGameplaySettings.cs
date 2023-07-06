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
    public class ModalGameplaySettings : BaseModal
    {
        [SerializeField] private CustomButton[] _selectButtons;

        public override UniTask Initialize(Memory<object> args)
        {
            GameManager.Instance.SetGameStateType(GameStateType.GameplayPausing, true);

            for (int i = 0; i < _selectButtons.Length; i++)
            {
                _selectButtons[i].Index = i;
                _selectButtons[i].CustomPointEnterAction = OnEnterAnItem;
            }

            _selectButtons[0].onClick.RemoveAllListeners();
            _selectButtons[0].onClick.AddListener(OnResume);

            _selectButtons[1].onClick.RemoveAllListeners();
            _selectButtons[1].onClick.AddListener(OnOpenControl);

            _selectButtons[2].onClick.RemoveAllListeners();
            _selectButtons[2].onClick.AddListener(OnReplay);

            _selectButtons[3].onClick.RemoveAllListeners();
            _selectButtons[3].onClick.AddListener(OnBackHome);

            _selectButtons[4].onClick.RemoveAllListeners();
            _selectButtons[4].onClick.AddListener(OnExit);

            EnterAButton(_selectButtons[0]);
            currentSelectedIndex = 0;
            return base.Initialize(args);
        }

        public override UniTask Cleanup()
        {
            GameManager.Instance.ReturnPreviousGameState();
            return base.Cleanup();
        }

        private void OnExit()
        {
            var content = LocalizeManager.GetLocalize(LocalizeTable.UI, LocalizeKeys.POPUP_CONFIRM_QUIT_GAME);
            var windowOptions = new WindowOptions(ModalIds.CONFIRM_ACTION);
            ScreenNavigator.Instance.LoadModal(windowOptions, new ModalConfirmActionData(content, () => Application.Quit())).Forget();
        }

        private void OnBackHome()
        {
            var content = LocalizeManager.GetLocalize(LocalizeTable.UI, LocalizeKeys.POPUP_CONFIRM_BACK_HOME);
            var windowOptions = new WindowOptions(ModalIds.CONFIRM_ACTION);
            ScreenNavigator.Instance.LoadModal(windowOptions, new ModalConfirmActionData(content, () => GameManager.Instance.BackHomeAsync().Forget())).Forget();
        }

        private void OnReplay()
        {
            var content = LocalizeManager.GetLocalize(LocalizeTable.UI, LocalizeKeys.POPUP_CONFIRM_REPLAY);
            var windowOptions = new WindowOptions(ModalIds.CONFIRM_ACTION);
            ScreenNavigator.Instance.LoadModal(windowOptions, new ModalConfirmActionData(content, () => GameManager.Instance.Replay().Forget())).Forget();
        }

        private void OnResume()
        {
            ScreenNavigator.Instance.PopModal(true).Forget();
        }

        private void OnOpenControl()
        {
            var windowOptions = new WindowOptions(ModalIds.GAMEPLAY_CONTROL_SETTINGS);
            ScreenNavigator.Instance.LoadModal(windowOptions).Forget();
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