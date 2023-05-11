using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public abstract class CharacterLevelConfigItem : BaseWithPointConfigItem
    {
        public uint level;
        public int detectedPriority;
        public abstract CharacterLevelStats CharacterLevelStats { get; }
    }

    [Serializable]
    public abstract class CharacterConfigItem<T> where T : CharacterLevelConfigItem
    {
        #region Members

        public uint id;
        public T[] levels;

        #endregion Members
    }

    [Serializable]
    public class CharacterLevelStats
    {
        public float detectRange;
        public float hp;
        public float armor;
        public float armorPenetration;
        public float critChance;
        public float critDamage;
        public float moveSpeed;
        public float lifeSteal;
        public float attackDamage;
    }
}