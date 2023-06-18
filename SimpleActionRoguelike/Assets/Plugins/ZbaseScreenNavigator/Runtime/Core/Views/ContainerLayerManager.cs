using System;
using System.Collections.Generic;
using UnityEngine;
using ZBase.UnityScreenNavigator.Foundation;

namespace ZBase.UnityScreenNavigator.Core.Views
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform), typeof(Canvas))]
    public class ContainerLayerManager : View, IContainerLayerManager
    {
        private readonly List<IContainerLayer> _containerLayers = new();

        public IReadOnlyList<IContainerLayer> ContainerLayers => _containerLayers;

        public void Add(IContainerLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException(nameof(layer));

            if (_containerLayers.Contains(layer))
                return;

            _containerLayers.Add(layer);

            if (layer.TryGetTransform(out var layerTransform))
                transform.AddChild(layerTransform);
        }

        public bool Remove(IContainerLayer layer)
        {
            if (layer == null)
                throw new ArgumentNullException(nameof(layer));

            return _containerLayers.Remove(layer);
        }

        public T Find<T>() where T : IContainerLayer
        {
            if (TryFind<T>(out var layer))
                return layer;

            Debug.LogError($"Cannot find layer of type {typeof(T).Name}");
            return default;
        }

        public T Find<T>(string layerName) where T : IContainerLayer
        {
            if (TryFind<T>(layerName, out var layer))
                return layer;

            Debug.LogError($"Cannot find layer {layerName}");
            return default;
        }

        public bool TryFind<T>(out T containerLayer) where T : IContainerLayer
        {
            containerLayer = default;

            var count = _containerLayers.Count;

            for (var i = 0; i < count; i++)
            {
                var layer = _containerLayers[i];

                if (layer == null)
                    continue;

                if (layer is T layerT)
                {
                    containerLayer = layerT;
                    break;
                }
            }

            return containerLayer != null;
        }

        public bool TryFind<T>(string layerName, out T containerLayer) where T : IContainerLayer
        {
            containerLayer = default;

            var count = _containerLayers.Count;

            for (var i = 0; i < count; i++)
            {
                var layer = _containerLayers[i];

                if (layer == null)
                    continue;

                if (string.Equals(layer.LayerName, layerName) && layer is T layerT)
                {
                    containerLayer = layerT;
                    break;
                }
            }

            return containerLayer != null;
        }

        public void Clear()
        {
            for (var i = _containerLayers.Count - 1; i >= 0; i--)
            {
                var item = _containerLayers[i];
                _containerLayers.RemoveAt(i);

                if (item.TryGetTransform(out var transform))
                    Destroy(transform.gameObject);
            }

            _containerLayers.Clear();
        }
    }
}