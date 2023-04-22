using Runtime.Definition;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class EnemyLevelStats : CharacterLevelStats 
    {
        public float collideDamage;
    }

    [Serializable]
    public class EnemyLevelConfigItem : CharacterLevelConfigItem
    {
        #region Members

        public EnemyLevelStats zombieLevelStats;
        public SkillIdentity skillIdentity;

        #endregion Members

        #region Properties

        public override CharacterLevelStats CharacterLevelStats => zombieLevelStats;

        #endregion Properties
    }

    [Serializable]
    public class EnemyConfigItem : CharacterConfigItem<EnemyLevelConfigItem> { }

    public class EnemyConfig : BaseConfig<EnemyConfigItem> { }


    [Serializable]
    public struct SkillIdentity
    {
        public SkillType skillType;
        public int skillDataId;
    }

}