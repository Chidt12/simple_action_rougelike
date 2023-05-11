using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Singleton;
using System;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core;
using ZBase.UnityScreenNavigator.Core.Activities;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Views;
using ZBase.UnityScreenNavigator.Foundation;

namespace Runtime.UI
{
    public class ContainerKey
    {
        public const string SCREEN_CONTAINER_LAYER_NAME = "ScreensContainer";
        public const string MODAL_CONTAINER_LAYER_NAME = "ModalsContainer";
    }

    public class ScreenNavigator : PersistentMonoSingleton<ScreenNavigator>
    {
        [SerializeField]
        protected ContainerLayerSettings containerLayerSettings;
        [SerializeField]
        protected UnityScreenNavigatorSettings unityScreenNavigatorSettings;

        protected GlobalContainerLayerManager globalContainerLayerManager;

        protected bool isLoading;
        protected bool isBackKeyOperated;
        protected bool isSceneTransitioned;

        protected bool IsOpeningAModal
        {
            get
            {
                var modalContainer = globalContainerLayerManager.Find<ModalContainer>(ContainerKey.MODAL_CONTAINER_LAYER_NAME);
                return modalContainer.Modals.Count > 0;
            }
        }

        protected bool IsOpeningMoreThanAScreen
        {
            get
            {
                var screenContainer = globalContainerLayerManager.Find<ScreenContainer>(ContainerKey.SCREEN_CONTAINER_LAYER_NAME);
                return screenContainer.Screens.Count > 1;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (containerLayerSettings == null)
                throw new ArgumentNullException(nameof(containerLayerSettings));
            globalContainerLayerManager = this.GetOrAddComponent<GlobalContainerLayerManager>();

            var layers = containerLayerSettings.GetContainerLayers();
            var manager = globalContainerLayerManager;

            foreach (var layer in layers)
            {
                switch (layer.layerType)
                {
                    case ContainerLayerType.Modal:
                        ModalContainer.Create(layer, manager, unityScreenNavigatorSettings);
                        break;

                    case ContainerLayerType.Screen:
                        ScreenContainer.Create(layer, manager, unityScreenNavigatorSettings);
                        break;

                    case ContainerLayerType.Activity:
                        ActivityContainer.Create(layer, manager, unityScreenNavigatorSettings);
                        break;
                }
            }

            LoadScreen(new WindowOptions(ScreenIds.START_GAME)).Forget();
        }

        public async UniTask LoadModal(WindowOptions option, params object[] args)
        {
            if (isLoading)
                return;
            var modalContainer = globalContainerLayerManager.Find<ModalContainer>(ContainerKey.MODAL_CONTAINER_LAYER_NAME);
            if (modalContainer.Modals.Count == 0 || (option.resourcePath != modalContainer.Current.ResourcePath))
            {
                isLoading = true;
                await modalContainer.PushAsync(option, args);
                isLoading = false;
            }
        }

        public async UniTask PopModal(bool playAnimation, params object[] args)
        {
            if (isLoading)
                return;

            isLoading = true;
            var screenContainer = globalContainerLayerManager.Find<ModalContainer>(ContainerKey.MODAL_CONTAINER_LAYER_NAME);
            if (screenContainer.Modals.Count > 0)
                await screenContainer.PopAsync(playAnimation, args);
            isLoading = false;
        }

        public async UniTask LoadScreen(WindowOptions option, params object[] args)
        {
            if (isLoading)
                return;

            var screenContainer = globalContainerLayerManager.Find<ScreenContainer>(ContainerKey.SCREEN_CONTAINER_LAYER_NAME);
            if (screenContainer.Screens.Count == 0 || option.resourcePath != screenContainer.Current.ResourcePath)
            {
                isLoading = true;
                var modalContainer = globalContainerLayerManager.Find<ModalContainer>(ContainerKey.MODAL_CONTAINER_LAYER_NAME);
                while (modalContainer.Modals.Count > 0)
                    await modalContainer.PopAsync(false);
                await screenContainer.PushAsync(option, args);
                isLoading = false;
            }
        }

        public async UniTask PopToRootScreen(bool playAnimation, params object[] args)
        {
            if (isLoading)
                return;

            isLoading = true;
            var modalContainer = globalContainerLayerManager.Find<ModalContainer>(ContainerKey.MODAL_CONTAINER_LAYER_NAME);
            while (modalContainer.Modals.Count > 0)
                await modalContainer.PopAsync(false, args);
            var screenContainer = globalContainerLayerManager.Find<ScreenContainer>(ContainerKey.SCREEN_CONTAINER_LAYER_NAME);
            while (screenContainer.Screens.Count > 1)
                await screenContainer.PopAsync(false, args);
            isLoading = false;
        }

        public async UniTask PopScreen(bool playAnimation, params object[] args)
        {
            if (isLoading)
                return;
            isLoading = true;
            var screenContainer = globalContainerLayerManager.Find<ScreenContainer>(ContainerKey.SCREEN_CONTAINER_LAYER_NAME);
            if (screenContainer.Screens.Count > 1)
                await screenContainer.PopAsync(playAnimation, args);
            isLoading = false;
        }

        public async UniTask CloseAllModals()
        {
            isLoading = true;
            var modalContainer = globalContainerLayerManager.Find<ModalContainer>(ContainerKey.MODAL_CONTAINER_LAYER_NAME);
            while (modalContainer.Modals.Count > 0)
                await modalContainer.PopAsync(false);
            isLoading = false;
        }

        public async UniTask CloseAllScreens()
        {
            isLoading = true;
            var screenContainer = globalContainerLayerManager.Find<ScreenContainer>(ContainerKey.SCREEN_CONTAINER_LAYER_NAME);
            while (screenContainer.Screens.Count > 0)
                await screenContainer.PopAsync(false);
            isLoading = false;
        }

        public async UniTask CleanAll()
        {
            await CloseAllModals();
            await CloseAllScreens();
        }
    }
}