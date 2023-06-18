using ZBase.UnityScreenNavigator.Core.Views;

namespace ZBase.UnityScreenNavigator.Core
{
    public readonly struct ViewRef<T> where T : View
    {
        public readonly PoolingPolicy PoolingPolicy;
        public readonly T View;
        public readonly string ResourcePath;

        public ViewRef(
              T view
            , string resourcePath
            , PoolingPolicy poolingPolicy
        )
        {
            PoolingPolicy = poolingPolicy;
            View = view;
            ResourcePath = resourcePath;
        }

        public void Deconstruct(out T view, out string resourcePath)
        {
            view = View;
            resourcePath = ResourcePath;
        }

        public void Deconstruct(
              out T view
            , out string resourcePath
            , out PoolingPolicy poolingPolicy
        )
        {
            view = View;
            resourcePath = ResourcePath;
            poolingPolicy = PoolingPolicy;
        }

        public static implicit operator ViewRef(ViewRef<T> value)
            => new ViewRef(value.View, value.ResourcePath, value.PoolingPolicy);
    }

    public readonly struct ViewRef
    {
        public readonly PoolingPolicy PoolingPolicy;
        public readonly View View;
        public readonly string ResourcePath;

        public ViewRef(
              View view
            , string resourcePath
            , PoolingPolicy poolingPolicy
        )
        {
            PoolingPolicy = poolingPolicy;
            View = view;
            ResourcePath = resourcePath;
        }

        public void Deconstruct(out View view, out string resourcePath)
        {
            view = View;
            resourcePath = ResourcePath;
        }

        public void Deconstruct(
              out View view
            , out string resourcePath
            , out PoolingPolicy poolingPolicy
        )
        {
            view = View;
            resourcePath = ResourcePath;
            poolingPolicy = PoolingPolicy;
        }
    }
}
