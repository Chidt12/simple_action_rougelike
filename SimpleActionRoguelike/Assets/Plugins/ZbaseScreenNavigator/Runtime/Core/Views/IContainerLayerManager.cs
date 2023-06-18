using System.Collections.Generic;

namespace ZBase.UnityScreenNavigator.Core.Views
{
    /// <summary>
    /// Manages layers of UI views.
    /// </summary>
    public interface IContainerLayerManager
    {
        IReadOnlyList<IContainerLayer> ContainerLayers { get; }

        void Add(IContainerLayer layer);

        bool Remove(IContainerLayer window);

        T Find<T>() where T : IContainerLayer;

        T Find<T>(string layerName) where T : IContainerLayer;

        bool TryFind<T>(out T containerLayer) where T : IContainerLayer;

        bool TryFind<T>(string layerName, out T containerLayer) where T : IContainerLayer;

        void Clear();
    }
}