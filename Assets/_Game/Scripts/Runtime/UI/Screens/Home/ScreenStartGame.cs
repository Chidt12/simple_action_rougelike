using Cysharp.Threading.Tasks;
using Runtime.SceneLoading;
using System;
using UnityEngine;
using UnityEngine.UI;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace Runtime.UI
{
    public class ScreenStartGame : Screen
    {
        [SerializeField] Button _clickButton;

        public override UniTask Initialize(Memory<object> args)
        {
            _clickButton.onClick.AddListener(OnClick);
            return base.Initialize(args);
        }

        private void OnClick()
        {
            SceneLoaderManager.LoadSceneAsync("Gameplay").Forget();
        }
    }
}
