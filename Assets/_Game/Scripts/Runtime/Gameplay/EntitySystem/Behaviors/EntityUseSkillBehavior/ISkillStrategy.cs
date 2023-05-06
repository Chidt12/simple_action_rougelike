using Cysharp.Threading.Tasks;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public enum SkillPhase
    {
        Precheck,
        Cast,
        Backswing,
    }

    public struct SkillCancelResult
    {
        public readonly bool Result;
        public readonly SkillPhase CurrentPhase;

        public SkillCancelResult(bool result, SkillPhase currentPhase)
        {
            Result = result;
            CurrentPhase = currentPhase;
        }
    }

    public interface ISkillStrategy
    {
        public void Init(SkillModel skillModel, IEntityControlData creatorData);
        public void SetTriggerEventProxy(IEntityTriggerActionEventProxy entityTriggerActionEventProxy);
        public bool CheckCanUseSkill();
        public UniTask ExecuteAsync(CancellationToken cancellationToken, int index);
        public SkillCancelResult Cancel();
        public void Dispose();
    }
}
