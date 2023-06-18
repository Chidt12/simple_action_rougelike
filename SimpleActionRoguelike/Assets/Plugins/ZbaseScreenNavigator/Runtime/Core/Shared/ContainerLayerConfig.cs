using System;
using ZBase.UnityScreenNavigator.Foundation;

namespace ZBase.UnityScreenNavigator.Core
{
    [Serializable]
    public class ContainerLayerConfig
    {
        public string name;
        public ContainerLayerType layerType;

        public bool overrideSorting;

        [ShowIf(nameof(overrideSorting))]
        public SortingLayerId sortingLayer;

        [ShowIf(nameof(overrideSorting))]
        public int orderInLayer = 0;
    }
}
