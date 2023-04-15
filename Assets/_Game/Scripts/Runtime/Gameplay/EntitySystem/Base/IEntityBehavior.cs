using Cysharp.Threading.Tasks;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityBehavior
    {
        UniTask<bool> BuildAsync(IEntityData data);
    }

    public interface IUpdateEntityBehavior
    {
        void OnUpdate(float deltaTime);
    }

    public interface IDisposeEntityBehavior : IDisposable { }
}