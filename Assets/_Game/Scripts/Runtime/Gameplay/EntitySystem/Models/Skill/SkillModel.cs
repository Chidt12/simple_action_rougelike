namespace Runtime.Gameplay.EntitySystem
{
    public enum SkillTargetType
    {
        None = 0,
        Self = 1,
        All = 2,
        Opponent = 3,
        Ally = 4
    }

    public abstract class SkillModel
    {
        public float CastRange { get; protected set; }
        public float CurrentCooldown { get; set; }
        public float Cooldown { get; set; }
        public bool IsUsing { get; set; }
        public bool IsReady => CurrentCooldown <= 0;
        public SkillTargetType TargetType { get; protected set; }
        public bool CanBeCanceled { get; protected set; }


        public SkillModel(float castRange, float cooldown, SkillTargetType targetType, bool canBeCanceled = true)
        {
            TargetType = targetType;
            CastRange = castRange;
            Cooldown = cooldown;
            CanBeCanceled = canBeCanceled;
        }
    }
}
