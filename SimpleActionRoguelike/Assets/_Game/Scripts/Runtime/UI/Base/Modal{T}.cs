using Cysharp.Threading.Tasks;
using System;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace Runtime.UI
{
    public abstract class Modal<T> : Modal where T : class
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