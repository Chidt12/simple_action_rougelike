namespace Runtime.Definition
{
    public static class ActionInputTypeExtension
    {
        #region Class Methods

        public static int GetSkillIndex(this ActionInputType actionInputType)
        {
            var skillIndex = 0;
            if (actionInputType == ActionInputType.UseSkill1)
                skillIndex = 0;
            else if (actionInputType == ActionInputType.UseSkill2)
                skillIndex = 1;
            else if (actionInputType == ActionInputType.UseSkill3)
                skillIndex = 2;
            return skillIndex;
        }

        public static int GetAttackIndex(this ActionInputType actionInputType)
        {
            var attackIndex = 0;
            if (actionInputType == ActionInputType.Attack)
                attackIndex = 0;
            else if (actionInputType == ActionInputType.Attack1)
                attackIndex = 1;
            return attackIndex;
        }

        #endregion Class Methods
    }

    public enum ActionInputType
    {
        Attack = 0,
        Attack1 = 1,
        UseSkill1 = 2,
        UseSkill2 = 3,
        UseSkill3 = 4,
        Dash = 100,
        StartUseSkill = 101,
    }
}
