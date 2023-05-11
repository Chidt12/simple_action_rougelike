using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public enum SkillPhase
    {
        Ready,
        Precheck,
        Cast,
        Backswing,
        Cooldown
    }

    public static class SkillPhaseExtensions
    {
        public static bool IsCast(this SkillPhase skillPhase)
        {
            switch (skillPhase)
            {
                case SkillPhase.Cast:
                case SkillPhase.Backswing:
                    return true;
                default:
                    return false;
            }
        }
    }

    public struct SkillCancelResult
    {
        public readonly bool Result;

        public SkillCancelResult(bool result)
        {
            Result = result;
        }
    }

    public interface ISkillStrategy
    {
        public void Init(SkillModel skillModel, IEntityControlData creatorData);
        public void SetTriggerEventProxy(IEntityTriggerActionEventProxy entityTriggerActionEventProxy);
        public bool CheckCanUseSkill();
        public UniTask ExecuteAsync(CancellationToken cancellationToken, int index, Func<UniTask> finishedPrecheckAction = null);
        public SkillCancelResult Cancel();
        public void Dispose();
    }
}
