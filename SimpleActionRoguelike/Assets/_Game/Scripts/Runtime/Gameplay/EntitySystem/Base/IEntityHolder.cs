using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityHolder
    {
        IEntityData EntityData { get; }

        bool IsProxy { get; }

        UniTask<bool> BuildAsync(IEntityData entityModel);

        void Dispose();
    }
}
