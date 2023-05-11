using Cysharp.Threading.Tasks;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public interface ISkillAction
    {
        public void Init(IEntityData creatorData);
        public UniTask<bool> RunOperateAsync(CancellationToken cancellationToken);
        public void Cancel();
    }
}