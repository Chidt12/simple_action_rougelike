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

        public EnemyLevelStats enemyLevelStats;
        public SkillIdentity skillIdentity;
        public AutoInputStrategyType autoInputStrategy;
        public DeathDataIdentity deathDataIdentity;

        #endregion Members

        #region Properties

        public override CharacterLevelStats CharacterLevelStats => enemyLevelStats;

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
        public int skillAnimIndex;
    }

}