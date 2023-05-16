using Cysharp.Threading.Tasks;
using Runtime.Constants;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZBase.Foundation.PubSub;
using ZBase.UnityScreenNavigator.Core.Views;
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
            ScreenNavigator.Instance.LoadModal(new WindowOptions(ModalIds.SELECT_INGAME_SHOP)).Forget();
        }
    }
}
