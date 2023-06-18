﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Views;
using ZBase.UnityScreenNavigator.Foundation;

namespace ZBase.UnityScreenNavigator.Core.Activities
{
    [RequireComponent(typeof(RectMask2D))]
    public class ActivityContainer : ContainerLayer
    {
        private static Dictionary<int, ActivityContainer> s_instanceCacheByTransform = new();
        private static Dictionary<string, ActivityContainer> s_instanceCacheByName = new();

        private readonly List<IActivityContainerCallbackReceiver> _callbackReceivers = new();
        private readonly Dictionary<string, ViewRef<Activity>> _activities = new();

        public IReadOnlyDictionary<string, ViewRef<Activity>> Activities => _activities;

        /// <seealso href="https://docs.unity3d.com/Manual/DomainReloading.html"/>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            s_instanceCacheByTransform = new();
            s_instanceCacheByName = new();
        }

        /// <summary>
        /// Get the <see cref="ActivityContainer" /> that manages the screen to which <see cref="transform" /> belongs.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="transform" />.</param>
        /// <returns></returns>
        public static ActivityContainer Of(Transform transform, bool useCache = true)
        {
            return Of((RectTransform)transform, useCache);
        }

        /// <summary>
        /// Get the <see cref="ActivityContainer" /> that manages the screen to which <see cref="rectTransform" /> belongs.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="rectTransform" />.</param>
        /// <returns></returns>
        public static ActivityContainer Of(RectTransform rectTransform, bool useCache = true)
        {
            var id = rectTransform.GetInstanceID();
            if (useCache && s_instanceCacheByTransform.TryGetValue(id, out var container))
            {
                return container;
            }

            container = rectTransform.GetComponentInParent<ActivityContainer>();

            if (container)
            {
                s_instanceCacheByTransform.Add(id, container);
                return container;
            }

            Debug.LogError($"Cannot find any parent {nameof(ActivityContainer)} component", rectTransform);
            return null;
        }

        /// <summary>
        /// Find the <see cref="ActivityContainer" /> of <see cref="containerName" />.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static ActivityContainer Find(string containerName)
        {
            if (s_instanceCacheByName.TryGetValue(containerName, out var instance))
            {
                return instance;
            }

            Debug.LogError($"Cannot find any {nameof(ActivityContainer)} by name `{containerName}`");
            return null;
        }

        /// <summary>
        /// Find the <see cref="ActivityContainer" /> of <see cref="containerName" />.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static bool TryFind(string containerName, out ActivityContainer container)
        {
            if (s_instanceCacheByName.TryGetValue(containerName, out var instance))
            {
                container = instance;
                return true;
            }

            Debug.LogError($"Cannot find any {nameof(ActivityContainer)} by name `{containerName}`");
            container = default;
            return false;
        }

        [Obsolete("This method is deprecated. Use Create(ContainerLayerConfig, IContainerLayerManager) instead")]
        public static async UniTask<ActivityContainer> CreateAsync(
              ContainerLayerConfig layerConfig
            , IContainerLayerManager manager
            , UnityScreenNavigatorSettings settings
        )
        {
            var container = Create(layerConfig, manager, settings);
            await UniTask.NextFrame();
            return container;
        }

        /// <summary>
        /// Create a new instance of <see cref="ActivityContainer"/> as a layer
        /// </summary>
        public static ActivityContainer Create(
              ContainerLayerConfig layerConfig
            , IContainerLayerManager manager
            , UnityScreenNavigatorSettings settings
        )
        {
            var root = new GameObject(
                  layerConfig.name
                , typeof(Canvas)
                , typeof(GraphicRaycaster)
                , typeof(CanvasGroup)
            );

            var rectTransform = root.GetOrAddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localPosition = Vector3.zero;

            var container = root.GetOrAddComponent<ActivityContainer>();
            container.Initialize(layerConfig, manager, settings);

            s_instanceCacheByName.Add(container.LayerName, container);
            return container;
        }

        /// <summary>
        /// Add a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void AddCallbackReceiver(IActivityContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Add(callbackReceiver);
        }

        /// <summary>
        /// Remove a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void RemoveCallbackReceiver(IActivityContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Remove(callbackReceiver);
        }

        private void Add(string resourcePath, Activity activity, PoolingPolicy poolingPolicy)
        {
            if (resourcePath == null)
                throw new ArgumentNullException(nameof(resourcePath));

            if (_activities.TryGetValue(resourcePath, out var viewRef))
            {
                if (activity != viewRef.View)
                {
                    Debug.LogWarning($"Another {nameof(Activity)} is existing for `{resourcePath}`", viewRef.View);
                }

                return;
            }

            viewRef = new ViewRef<Activity>(activity, resourcePath, poolingPolicy);
            _activities.Add(resourcePath, viewRef);

            if (activity.TryGetTransform(out var transform))
            {
                this.transform.AddChild(transform);
            }
        }

        private bool Remove(string resourcePath)
        {
            if (resourcePath == null)
                throw new ArgumentNullException(nameof(resourcePath));

            if (_activities.TryGetValue(resourcePath, out var activity))
            {
                if (activity.TryGetTransform(out var transform))
                {
                    this.transform.RemoveChild(transform);
                }

                return _activities.Remove(resourcePath);
            }

            return false;
        }

        public bool TryGet(string resourcePath, out ViewRef<Activity> activity)
        {
            return _activities.TryGetValue(resourcePath, out activity);
        }

        public bool TryGet<T>(string resourcePath, out T activity)
            where T : Activity
        {
            if (_activities.TryGetValue(resourcePath, out var otherActivity))
            {
                if (otherActivity is T activityT)
                {
                    activity = activityT;
                    return true;
                }
            }

            activity = default;
            return false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var (activity, resourcePath) in _activities.Values)
            {
                DestroyAndForget(new ViewRef(activity, resourcePath, PoolingPolicy.DisablePooling)).Forget();
            }

            _activities.Clear();
        }

        protected virtual int GetChildIndex(Transform child)
        {
            var myTransform = this.transform;
            var count = myTransform.childCount;

            for (var i = count - 1; i >= 0; i--)
            {
                if (myTransform.GetChild(i).Equals(child))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Show an instance of <typeparamref name="TActivity"/>.
        /// </summary>
        /// <remarks>Fire-and-forget</remarks>
        public void Show<TActivity>(ActivityOptions options, params object[] args)
            where TActivity : Activity
        {
            ShowAndForget<TActivity>(options, args).Forget();
        }

        /// <summary>
        /// Show an instance of <see cref="Activity"/>.
        /// </summary>
        /// <remarks>Fire-and-forget</remarks>
        public void Show(ActivityOptions options, params object[] args)
        {
            ShowAndForget<Activity>(options, args).Forget();
        }

        /// <summary>
        /// Show an instance of <typeparamref name="TActivity"/>.
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        public async UniTask ShowAsync<TActivity>(ActivityOptions options, params object[] args)
            where TActivity : Activity
        {
            await ShowAsyncInternal<TActivity>(options, args);
        }

        /// <summary>
        /// Show an instance of <see cref="Activity"/>.
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        public async UniTask ShowAsync(ActivityOptions options, params object[] args)
        {
            await ShowAsyncInternal<Activity>(options, args);
        }

        private async UniTask ShowAndForget<TActivity>(ActivityOptions options, Memory<object> args)
            where TActivity : Activity
        {
            await ShowAsyncInternal<TActivity>(options, args);
        }

        private async UniTask ShowAsyncInternal<TActivity>(ActivityOptions options, Memory<object> args)
            where TActivity : Activity
        {
            var resourcePath = options.options.resourcePath;

            if (resourcePath == null)
            {
                throw new ArgumentNullException(nameof(resourcePath));
            }

            if (_activities.TryGetValue(resourcePath, out var viewRef))
            {
                Debug.LogWarning(
                    $"Cannot transition because the {typeof(TActivity).Name} at `{resourcePath}` is already showing."
                    , viewRef.View
                );

                return;
            }
            
            if (Settings.EnableInteractionInTransition == false)
            {
                Interactable = false;
            }

            var activity = await GetViewAsync<TActivity>(options.options);
            Add(resourcePath, activity, options.options.poolingPolicy);
            options.options.onLoaded?.Invoke(activity, args);

            await activity.AfterLoadAsync(RectTransform, args);

            activity.SetSortingLayer(options.sortingLayer, options.orderInLayer);

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforeShow(activity, args);
            }

            await activity.BeforeEnterAsync(true, args);

            // Play Animation
            await activity.EnterAsync(true, options.options.playAnimation);

            // Postprocess
            activity.AfterEnter(true, args);

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterShow(activity, args);
            }

            if (Settings.EnableInteractionInTransition == false)
            {
                Interactable = true;
            }
        }

        /// <summary>
        /// Hide an instance of <see cref="Activity"/>.
        /// </summary>
        /// <remarks>Fire-and-forget</remarks>
        public void Hide(string resourcePath, bool playAnimation = true, params object[] args)
        {
            HideAndForget(resourcePath, playAnimation, args).Forget();
        }

        private async UniTaskVoid HideAndForget(string resourcePath, bool playAnimation, params object[] args)
        {
            await HideAsync(resourcePath, playAnimation, args);
        }

        /// <summary>
        /// Hide an instance of <see cref="Activity"/>.
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        public async UniTask HideAsync(string resourcePath, bool playAnimation = true, params object[] args)
        {
            if (TryGet(resourcePath, out var viewRef) == false)
            {
                Debug.LogError(
                    $"Cannot transition because there is no activity loaded " +
                    $"on the stack by the resource path `{resourcePath}`"
                );

                return;
            }

            if (Settings.EnableInteractionInTransition == false)
            {
                Interactable = false;
            }

            var activity = viewRef.View;
            activity.Settings = Settings;

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforeHide(activity, args);
            }

            await activity.BeforeEnterAsync(false, args);

            // Play Animation
            await activity.EnterAsync(false, playAnimation);

            // End Transition
            Remove(resourcePath);

            // Postprocess
            activity.AfterEnter(false, args);

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterHide(activity, args);
            }

            // Unload unused Activity
            await activity.BeforeReleaseAsync();

            DestroyAndForget(new ViewRef(activity, resourcePath, viewRef.PoolingPolicy)).Forget();

            if (Settings.EnableInteractionInTransition == false)
            {
                Interactable = true;
            }
        }

        /// <summary>
        /// Hide all instances of <see cref="Activity"/>.
        /// </summary>
        /// <remarks>Fire-and-forget</remarks>
        public void HideAll(bool playAnimation = true)
        {
            HideAllAndForget(playAnimation).Forget();
        }

        private async UniTaskVoid HideAllAndForget(bool playAnimation = true)
        {
            await HideAllAsync(playAnimation);
        }

        /// <summary>
        /// Hide all instances of <see cref="Activity"/>.
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        public async UniTask HideAllAsync(bool playAnimation = true)
        {
            var keys = Pool<List<string>>.Shared.Rent();
            keys.AddRange(_activities.Keys);

            var count = keys.Count;

            for (var i = 0; i < count; i++)
            {
                await HideAsync(keys[i], playAnimation);
            }

            Pool<List<string>>.Shared.Return(keys);
        }
    }
}