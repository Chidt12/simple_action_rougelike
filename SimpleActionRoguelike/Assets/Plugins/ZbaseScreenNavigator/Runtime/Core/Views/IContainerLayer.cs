using UnityEngine;

namespace ZBase.UnityScreenNavigator.Core.Views
{
    public interface IContainerLayer
    {
        string LayerName { get; }

        ContainerLayerType LayerType { get; }

        IContainerLayerManager ContainerLayerManager { get; }

        Canvas Canvas { get; }
    }
}