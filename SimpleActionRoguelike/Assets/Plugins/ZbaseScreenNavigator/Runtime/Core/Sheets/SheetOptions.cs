namespace ZBase.UnityScreenNavigator.Core.Sheets
{
    public readonly struct SheetOptions
    {
        public readonly bool loadAsync;
        public readonly string resourcePath;
        public readonly PoolingPolicy poolingPolicy;
        public readonly SheetLoadedAction onLoaded;

        public SheetOptions(
              string resourcePath
            , SheetLoadedAction onLoaded = null
            , bool loadAsync = true
            , PoolingPolicy poolingPolicy = PoolingPolicy.UseSettings
        )
        {
            this.loadAsync = loadAsync;
            this.resourcePath = resourcePath;
            this.onLoaded = onLoaded;
            this.poolingPolicy = poolingPolicy;
        }

        public static implicit operator SheetOptions(string resourcePath)
            => new(resourcePath);
    }
}