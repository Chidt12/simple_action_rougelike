using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class ThrowBombSkillDataConfigItem : SkillDataConfigItem
    {
        public int numberOfBombs;
        public float delayBetweenBombs;
        public float warningTime;
        public string warningPrefabName;
        public string impactPrefabName;
        public float impactWidth;
        public float impactHeight;
        public float damageBonus;
        public DamageFactor[] damageFactors;
        public bool random;
        public float offsetRandom;
    }

    public class ThrowBombSkillDataConfig : SkillDataConfig<ThrowBombSkillDataConfigItem>
    {

    }
}