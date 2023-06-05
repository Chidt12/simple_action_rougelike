using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Message;
using System;
using System.Collections.Generic;
using ZBase.Foundation.PubSub;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace Runtime.UI
{
    public class BaseScreen : Screen
    {
        protected List<ISubscription> subscriptions;

        public override UniTask Initialize(Memory<object> args)
        {
            subscriptions = new();
            subscriptions.Add(SimpleMessenger.Subscribe<InputKeyPressMessage>(OnKeyPress));
            return base.Initialize(args);
        }

        public override UniTask Cleanup()
        {
            if (subscriptions != null)
            {
                foreach (var subscription in subscriptions)
                    subscription.Dispose();
            }
            return base.Cleanup();
        }

        protected virtual void OnKeyPress(InputKeyPressMessage message) { }
    }

    public abstract class Screen<T> : BaseScreen where T : class
    {
        public async override UniTask Initialize(Memory<object> args)
        {
            await base.Initialize(args);

            var obj = args.Span[0] as T;

            await Initialize(obj);
        }

        public abstract UniTask Initialize(T data);
    }
}