using ZBase.UnityScreenNavigator.Core.Views;

namespace ZBase.UnityScreenNavigator.Core.Sheets
{
    public readonly struct SheetRef<T> where T : Sheet
    {
        public readonly PoolingPolicy PoolingPolicy;
        public readonly T Sheet;
        public readonly string ResourcePath;

        public SheetRef(
              T sheet
            , string resourcePath
            , PoolingPolicy poolingPolicy
        )
        {
            PoolingPolicy = poolingPolicy;
            Sheet = sheet;
            ResourcePath = resourcePath;
        }

        public void Deconstruct(out T sheet, out string resourcePath)
        {
            sheet = Sheet;
            resourcePath = ResourcePath;
        }

        public void Deconstruct(
              out T sheet
            , out string resourcePath
            , out PoolingPolicy poolingPolicy
        )
        {
            sheet = Sheet;
            resourcePath = ResourcePath;
            poolingPolicy = PoolingPolicy;
        }

        public static implicit operator SheetRef(SheetRef<T> value)
            => new SheetRef(value.Sheet, value.ResourcePath, value.PoolingPolicy);
    }

    public readonly struct SheetRef
    {
        public readonly PoolingPolicy PoolingPolicy;
        public readonly Sheet Sheet;
        public readonly string ResourcePath;

        public SheetRef(
              Sheet sheet
            , string resourcePath
            , PoolingPolicy poolingPolicy
        )
        {
            PoolingPolicy = poolingPolicy;
            Sheet = sheet;
            ResourcePath = resourcePath;
        }

        public void Deconstruct(out Sheet sheet, out string resourcePath)
        {
            sheet = Sheet;
            resourcePath = ResourcePath;
        }

        public void Deconstruct(
              out Sheet sheet
            , out string resourcePath
            , out PoolingPolicy poolingPolicy
        )
        {
            sheet = Sheet;
            resourcePath = ResourcePath;
            poolingPolicy = PoolingPolicy;
        }
    }
}
