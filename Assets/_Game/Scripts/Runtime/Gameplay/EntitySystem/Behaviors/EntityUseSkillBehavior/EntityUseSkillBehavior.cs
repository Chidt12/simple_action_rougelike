using Cysharp.Threading.Tasks;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityUseSkillBehavior : EntityBehavior<IEntityControlData>
    {
        private IEntityControlData _controlData;
        private ISkillStrategy[] _skillStrategies;
        private CancellationTokenSource[] _skillCooldownCancellationTokenSource;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            _controlData = data;
            return UniTask.FromResult(true);
        }
    }
}
