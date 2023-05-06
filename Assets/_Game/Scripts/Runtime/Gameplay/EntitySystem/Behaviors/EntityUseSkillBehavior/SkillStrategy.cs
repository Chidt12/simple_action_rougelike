using Cysharp.Threading.Tasks;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class SkillStrategy<T> : ISkillStrategy where T : SkillModel
    {
        protected IEntityTriggerActionEventProxy entityTriggerActionEventProxy;
        protected CancellationTokenSource cancellationTokenSource;
        protected SkillPhase currentSkillActionPhase;
        protected IEntityControlData creatorData;
        protected T ownerModel;

        public void SetTriggerEventProxy(IEntityTriggerActionEventProxy entityTriggerActionEventProxy)
        {
            if(entityTriggerActionEventProxy != null)
                this.entityTriggerActionEventProxy = entityTriggerActionEventProxy;
        }

        public void Init(SkillModel skillModel, IEntityControlData creatorData)
        {
            entityTriggerActionEventProxy = new DummyEntityTriggerActionEventProxy();
            ownerModel = skillModel as T;
            this.creatorData = creatorData;
            Init(ownerModel);
        }

        protected virtual void Init(T skillModel) { }

        public async UniTask ExecuteAsync(CancellationToken cancellationToken, int index)
        {
            currentSkillActionPhase = SkillPhase.Precheck;
            if (CheckCanUseSkill())
            {
                await PrecheckSkillAsync();

                var precasting = true;
                entityTriggerActionEventProxy.TriggerEvent(index.GetPrecastSkillByIndex(), endAction: _ => precasting = false);
                await UniTask.WaitUntil(() => !precasting, cancellationToken: cancellationToken);

                currentSkillActionPhase = SkillPhase.Cast;
                await PresentSkillAsync(cancellationToken, index);

                var backswinging = true;
                currentSkillActionPhase = SkillPhase.Backswing;
                entityTriggerActionEventProxy.TriggerEvent(index.GetBackswingSkillByIndex(), endAction: _ => backswinging = false);
                await UniTask.WaitUntil(() => !backswinging, cancellationToken: cancellationToken);
            }
        }

        protected virtual UniTask PrecheckSkillAsync() => UniTask.CompletedTask;
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

        public virtual bool CheckCanUseSkill()
        {
            if (ownerModel.DependTarget)
                return creatorData.Target != null && !creatorData.Target.IsDead;
            return true;
        }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
        }

    }
}
