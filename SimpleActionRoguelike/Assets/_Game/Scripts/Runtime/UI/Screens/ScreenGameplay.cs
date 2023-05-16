using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Manager.Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace Runtime.UI
{
    public class ScreenGameplay : Screen
    {
        [SerializeField] private Button _shopButton;

        public override UniTask Initialize(Memory<object> args)
        {
            _shopButton.onClick.AddListener(OnClickShop);
            return base.Initialize(args);
        }

        public override UniTask Cleanup()
        {
            return base.Cleanup();
        }

        private void OnClickShop()
        {
            GameplayManager.Instance.ShowShop();
        }
    }
}
