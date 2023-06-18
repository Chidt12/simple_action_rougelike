using ZBase.UnityScreenNavigator.Core.Views;

namespace ZBase.UnityScreenNavigator.Core.Modals
{
    public readonly struct ModalOptions
    {
        public readonly float? backdropAlpha;
        public readonly bool? closeWhenClickOnBackdrop;
        public readonly string modalBackdropResourcePath;
        public readonly WindowOptions options;

        public ModalOptions(
              in WindowOptions options
            , in float? backdropAlpha = null
            , in bool? closeWhenClickOnBackdrop = null
            , string modalBackdropResourcePath = null
        )
        {
            this.options = options;
            this.backdropAlpha = backdropAlpha;
            this.closeWhenClickOnBackdrop = closeWhenClickOnBackdrop;
            this.modalBackdropResourcePath = modalBackdropResourcePath;
        }

        public ModalOptions(
              string resourcePath
            , bool playAnimation = true
            , OnLoadCallback onLoaded = null
            , bool loadAsync = true
            , in float? backdropAlpha = null
            , in bool? closeWhenClickOnBackdrop = null
            , string modalBackdropResourcePath = null
            , PoolingPolicy poolingPolicy = PoolingPolicy.UseSettings
        )
        {
            this.options = new(resourcePath, playAnimation, onLoaded, loadAsync, poolingPolicy);
            this.backdropAlpha = backdropAlpha;
            this.closeWhenClickOnBackdrop = closeWhenClickOnBackdrop;
            this.modalBackdropResourcePath = modalBackdropResourcePath;
        }

        public static implicit operator ModalOptions(in WindowOptions options)
            => new(options);

        public static implicit operator ModalOptions(string resourcePath)
            => new(new WindowOptions(resourcePath));

        public static implicit operator WindowOptions(in ModalOptions options)
            => options.options;
    }
}
