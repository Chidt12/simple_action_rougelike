using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityBehavior
    {
        UniTask<bool> BuildAsync(IEntityData data, CancellationToken cancellationToken);
    }

    public interface IUpdateEntityBehavior
    {
        void OnUpdate(float deltaTime);
    }

    public interface IDisposeEntityBehavior : IDisposable { }
}