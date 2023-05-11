using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class HeroLevelStats : CharacterLevelStats
    {
        public float attackSpeed;
        public float dodgeChance;
        public float ccReduction;
        public float cooldownReduction;
        public float damageReduction;
    }

    [Serializable]
    public class HeroLevelConfigItem : CharacterLevelConfigItem
    {
        public HeroLevelStats heroLevelStats;

        public override CharacterLevelStats CharacterLevelStats => heroLevelStats;
    }

    [Serializable]
    public class HeroConfigItem : CharacterConfigItem<HeroLevelConfigItem> { }

    public class HeroConfig : BaseConfig<HeroConfigItem> { }
}