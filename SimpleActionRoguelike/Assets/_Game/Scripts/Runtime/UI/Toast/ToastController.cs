using Cysharp.Threading.Tasks;
using Runtime.Core.Singleton;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Runtime.UI
{
    public enum VisualToastType
    {
        Normal = 0,
    }

    public enum PositionToastType
    {
        Middle = 0,
        Top = 1,
        Bottom = 2,
    }

    public class ToastController : MonoSingleton<ToastController>
    {
        [SerializeField] private Transform _bottomPosition;
        [SerializeField] private Transform _topPosition;
        [SerializeField] private Transform _middlePosition;

        private Dictionary<VisualToastType, ToastQueue> _toastQueueDictionary;

        protected override void Awake()
        {
            base.Awake();
            _toastQueueDictionary = new();
        }

        public void Show(string content, PositionToastType positionToastType = PositionToastType.Top, VisualToastType visualToastType = VisualToastType.Normal)
           => LoadToastAsync(content, positionToastType, visualToastType).Forget();

        private async UniTask LoadToastAsync(string content, PositionToastType positionToastType = PositionToastType.Middle, VisualToastType visualToastType = VisualToastType.Normal)
        {
            var result = _toastQueueDictionary.TryGetValue(visualToastType, out var toastQueue);
            if (!result)
            {
                var prefabId = $"toast_{visualToastType.ToString().ToLower()}";
                var prefab = await Addressables.LoadAssetAsync<GameObject>(prefabId).WithCancellation(this.GetCancellationTokenOnDestroy());
                toastQueue = new ToastQueue(prefab.GetComponent<Toast>());
                _toastQueueDictionary.TryAdd(visualToastType, toastQueue);
            }

            var dequeueResult = toastQueue.queue.TryDequeue(out var toast);
            if (!dequeueResult)
                toast = Instantiate(toastQueue.prefab);

            Transform positionTransform;
            switch (positionToastType)
            {
                case PositionToastType.Middle:
                    positionTransform = _middlePosition;
                    break;
                case PositionToastType.Top:
                    positionTransform = _topPosition;
                    break;
                case PositionToastType.Bottom:
                    positionTransform = _bottomPosition;
                    break;
                default:
                    positionTransform = _middlePosition;
                    break;
            }

            toast.Init(content, positionTransform, ReturnPool);
        }

        private void ReturnPool(Toast toast)
        {
            var result = _toastQueueDictionary.TryGetValue(toast.VisualToastType, out var toastQueue);
            if (result)
                toastQueue.queue.Enqueue(toast);
        }

        private class ToastQueue
        {
            public Toast prefab;
            public Queue<Toast> queue;

            public ToastQueue(Toast prefab)
            {
                this.prefab = prefab;
                queue = new Queue<Toast>();
            }
        }
    }
}
