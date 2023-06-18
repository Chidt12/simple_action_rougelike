namespace Runtime.Gameplay.EntitySystem
{
    public enum AnimationType
    {
        None = 0,
        Idle = 1,
        Run = 2,
        Death = 3,
        Attack1 = 4,
        Attack2 = 5,
        PrecastSkill1 = 6,
        PrecastSkill2 = 7,
        PrecastSkill3 = 8,
        UseSkill1 = 9,
        UseSkill2 = 10,
        UseSkill3 = 11,
        BackswingSkill1 = 12,
        BackswingSkill2 = 13,
        BackswingSkill3 = 14,
        GetHurt = 15,

        PrecastSkill4 = 16,
        UseSkill4 = 17,
        BackswingSkill4 = 18,
        
        UseSkillPart1 = 19,
        UseSkillPart2 = 20,
        UseSkillPart3 = 21,
        UseSkillPart4 = 22,

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
            else if (index == 3)
                precast = AnimationType.PrecastSkill4;

            return precast;
        }

        public static AnimationType GetBackswingSkillByIndex(this int index)
        {
            AnimationType backswing = AnimationType.BackswingSkill1;
            if (index == 1)
                backswing = AnimationType.BackswingSkill2;
            else if (index == 2)
                backswing = AnimationType.BackswingSkill3;
            else if (index == 3)
                backswing = AnimationType.BackswingSkill4;

            return backswing;
        }

        public static AnimationType GetUseSkillByIndex(this int index)
        {
            AnimationType useSkill = AnimationType.UseSkill1;
            if (index == 1)
                useSkill = AnimationType.UseSkill2;
            else if (index == 2)
                useSkill = AnimationType.UseSkill3;
            else if (index == 3)
                useSkill = AnimationType.UseSkill4;

            return useSkill;
        }

        public static AnimationType GetUseSkillPartByIndex(this int index)
        {
            AnimationType useSkill = AnimationType.UseSkillPart1;
            if (index == 1)
                useSkill = AnimationType.UseSkillPart2;
            else if (index == 2)
                useSkill = AnimationType.UseSkillPart3;
            else if (index == 3)
                useSkill = AnimationType.UseSkillPart4;

            return useSkill;
        }
    }
}
