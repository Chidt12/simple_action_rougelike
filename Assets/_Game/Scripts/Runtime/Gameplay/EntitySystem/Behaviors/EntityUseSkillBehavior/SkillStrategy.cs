using Cysharp.Threading.Tasks;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class SkillStrategy<T> : ISkillStrategy where T : SkillModel
    {
        protected IEntityTriggerActionEventProxy entityTriggerActionEventProxy;
        protected CancellationTokenSource cancellationTokenSource;
        protected SkillPhase currentSkillActionPhase;
        protected IEntityData creatorData;
        protected T ownerModel;

        public void SetTriggerEventProxy(IEntityTriggerActionEventProxy entityTriggerActionEventProxy)
        {
            this.entityTriggerActionEventProxy = entityTriggerActionEventProxy;
        }

        public void Init(SkillModel skillModel, IEntityData creatorData)
        {
            ownerModel = skillModel as T;
            this.creatorData = creatorData;
            Init(ownerModel);
        }

        public abstract void Init(T skillModel);

        public async UniTask ExecuteAsync(CancellationToken cancellationToken, int index)
        {
            currentSkillActionPhase = SkillPhase.Precheck;
            await entityTriggerActionEventProxy.TriggerEvent(index.GetPrecastSkillByIndex(), cancellationToken);
            currentSkillActionPhase = SkillPhase.Cast;
            await PresentSkillAsync(cancellationToken, index);
            currentSkillActionPhase = SkillPhase.Backswing;
            await entityTriggerActionEventProxy.TriggerEvent(index.GetBackswingSkillByIndex(), cancellationToken);
        }

        protected abstract UniTask PresentSkillAsync(CancellationToken cancellationToken, int index);

        public SkillCancelResult Cancel()
        {
            if (ownerModel.CanBeCanceled)
            {
                cancellationTokenSource?.Cancel();
                return new SkillCancelResult(true, currentSkillActionPhase);
            }
            else return new SkillCancelResult(false, currentSkillActionPhase);
        }

        public abstract bool CheckCanUseSkill();

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
        }

    }
}
