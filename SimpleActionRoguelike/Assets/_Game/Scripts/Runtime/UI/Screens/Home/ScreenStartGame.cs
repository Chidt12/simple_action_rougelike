using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Manager;
using Runtime.Message;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Views;

namespace Runtime.UI
{
    public class ScreenStartGame : BaseScreen
    {
        [SerializeField] private Button _startButton;

        public override UniTask Initialize(Memory<object> args)
        {
            _startButton.onClick.RemoveAllListeners();
            _startButton.onClick.AddListener(() => {
                var windowOptions = new WindowOptions(ScreenIds.HOME);
                ScreenNavigator.Instance.LoadScreen(windowOptions).Forget();
            });
            return base.Initialize(args);
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            if (message.KeyPressType == KeyPressType.Confirm)
            {
                Submit(_startButton);
            }
        }
    }
}
