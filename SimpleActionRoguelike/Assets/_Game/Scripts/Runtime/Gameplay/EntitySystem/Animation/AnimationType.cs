namespace Runtime.Gameplay.EntitySystem
{
    public enum AnimationType
    {
        None,
        Idle,
        Run,
        Death,
        Attack1,
        Attack2,
        PrecastSkill1,
        PrecastSkill2,
        PrecastSkill3,
        UseSkill1,
        UseSkill2,
        UseSkill3,
        BackswingSkill1,
        BackswingSkill2,
        BackswingSkill3,
        GetHurt,
    }

    public static class AnimationTypeExtensions
    {
        public static AnimationType GetPrecastSkillByIndex(this int index)
        {
            AnimationType precast = AnimationType.PrecastSkill1;
            if (index == 1)
                precast = AnimationType.PrecastSkill2;
            else if (index == 2)
                precast = AnimationType.PrecastSkill3;

            return precast;
        }

        public static AnimationType GetBackswingSkillByIndex(this int index)
        {
            AnimationType backswing = AnimationType.BackswingSkill1;
            if (index == 1)
                backswing = AnimationType.BackswingSkill1;
            else if (index == 2)
                backswing = AnimationType.BackswingSkill1;

            return backswing;
        }

        public static AnimationType GetUseSkillByIndex(this int index)
        {
            AnimationType useSkill = AnimationType.UseSkill1;
            if (index == 1)
                useSkill = AnimationType.UseSkill1;
            else if (index == 2)
                useSkill = AnimationType.UseSkill1;

            return useSkill;
        }
    }
}
