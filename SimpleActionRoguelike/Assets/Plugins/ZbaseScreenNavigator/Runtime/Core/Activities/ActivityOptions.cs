using ZBase.UnityScreenNavigator.Core.Views;
using ZBase.UnityScreenNavigator.Foundation;

namespace ZBase.UnityScreenNavigator.Core.Activities
{
    public readonly struct ActivityOptions
    {
        public readonly SortingLayerId? sortingLayer;
        public readonly int? orderInLayer;
        public readonly WindowOptions options;

        public ActivityOptions(
              in WindowOptions options
            , in SortingLayerId? sortingLayer = null
            , in int? orderInLayer = null
        )
        {
            this.options = options;
            this.sortingLayer = sortingLayer;
            this.orderInLayer = orderInLayer;
        }

        public ActivityOptions(
              string resourcePath
            , bool playAnimation = true
            , OnLoadCallback onLoaded = null
            , bool loadAsync = true
            , in SortingLayerId? sortingLayer = null
            , in int? orderInLayer = null
            , PoolingPolicy poolingPolicy = PoolingPolicy.UseSettings
        )
        {
            this.options = new(resourcePath, playAnimation, onLoaded, loadAsync, poolingPolicy);
            this.sortingLayer = sortingLayer;
            this.orderInLayer = orderInLayer;
        }

        public static implicit operator ActivityOptions(in WindowOptions options)
            => new(options);

        public static implicit operator ActivityOptions(string resourcePath)
            => new(new WindowOptions(resourcePath));

        public static implicit operator WindowOptions(in ActivityOptions options)
            => options.options;
    }
}
