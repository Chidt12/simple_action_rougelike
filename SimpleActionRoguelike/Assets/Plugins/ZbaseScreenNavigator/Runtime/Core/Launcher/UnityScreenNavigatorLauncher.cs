using System;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Activities;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Views;
using ZBase.UnityScreenNavigator.Foundation;

namespace ZBase.UnityScreenNavigator.Core
{
    [RequireComponent(typeof(RectTransform), typeof(Canvas))]
    public class UnityScreenNavigatorLauncher : MonoBehaviour
    {
        [SerializeField]
        private UnityScreenNavigatorSettings unityScreenNavigatorSettings;

        [SerializeField]
        private ContainerLayerSettings containerLayerSettings;

        protected GlobalContainerLayerManager GlobalContainerLayerManager { get; private set; }

        protected virtual void Awake()
        {
            if (unityScreenNavigatorSettings == false)
            {
                throw new NullReferenceException(nameof(unityScreenNavigatorSettings));
            }

            if (containerLayerSettings == false)
            {
                throw new NullReferenceException(nameof(containerLayerSettings));
            }

            UnityScreenNavigatorSettings.DefaultSettings = unityScreenNavigatorSettings;
            GlobalContainerLayerManager = this.GetOrAddComponent<GlobalContainerLayerManager>();
        }

        protected virtual void Start()
        {
            var layers = containerLayerSettings.GetContainerLayers();
            var manager = GlobalContainerLayerManager;

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
        }
    }
}