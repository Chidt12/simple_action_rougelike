using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Manager;
using Runtime.Manager.Data;
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
                if (!GameManager.Instance.IsTest)
                {
                    if(!DataManager.Local.playerBasicLocalData.CheckCompletedTut(TutorialType.GuideGameplay))
                    {

                    }
                    else
                    {
                        var windowOptions = new WindowOptions(ScreenIds.HOME);
                        ScreenNavigator.Instance.LoadSingleScreen(windowOptions, true).Forget();
                    }
                }
                else
                {
                    GameManager.Instance.StartLoadingGameplayAsync().Forget();
                }
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
