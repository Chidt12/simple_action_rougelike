using Cysharp.Threading.Tasks;
using Runtime.Manager;
using Runtime.Message;
using System;

namespace Runtime.UI
{
    public class ScreenStartGame : BaseScreen
    {
        public override UniTask Initialize(Memory<object> args)
        {
            return base.Initialize(args);
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            if (message.KeyPressType == KeyPressType.Confirm)
            {
                GameManager.Instance.StartLoadingGameplayAsync().Forget();
            }
        }
    }
}
