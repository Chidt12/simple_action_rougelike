using Cysharp.Threading.Tasks;
using Runtime.Manager.Gameplay;
using System;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace Runtime.UI
{
    public class ScreenGameplay : Screen
    {
        public override UniTask Initialize(Memory<object> args)
        {
            return base.Initialize(args);
        }

        public override UniTask Cleanup()
        {
            return base.Cleanup();
        }
    }
}
