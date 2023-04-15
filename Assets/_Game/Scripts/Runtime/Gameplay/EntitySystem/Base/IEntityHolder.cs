using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityHolder
    {
        UniTask<bool> BuildAsync(EntityModel entityModel);

        void Dispose();
    }
}
