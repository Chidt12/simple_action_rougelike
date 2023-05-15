using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBase.Foundation.PubSub;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace Runtime.UI
{
    public class ScreenGameplay : Screen
    {
        [SerializeField] private Button _shopButton;
        [SerializeField] private TextMeshProUGUI _currentGold;

        private ISubscription _subScriptions;

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

        }
    }
}
