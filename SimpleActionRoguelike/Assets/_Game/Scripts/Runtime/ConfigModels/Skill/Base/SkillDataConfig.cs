using Runtime.Definition;
using System;
using System.Linq;
using UnityEngine;

namespace Runtime.ConfigModel
{
    [Serializable]
    public abstract class SkillDataConfigItem
    {
        #region Members

        public int dataId;
        public SkillTargetType targetType;
        public float castRange;
        public float cooldown;
        public bool dependTarget;
        public bool canBeCanceled;

        #endregion Members
    }

    public abstract class SkillDataConfig : ScriptableObject
    {
        #region Properties

        public virtual SkillDataConfigItem[] Items { get; }

        #endregion Properties
    }

    public abstract class SkillDataConfig<T> : SkillDataConfig where T : SkillDataConfigItem
    {
        #region Members

        public T[] items;

        #endregion Members

        #region Properties

        public override SkillDataConfigItem[] Items
            => items.Cast<SkillDataConfigItem>().ToArray();

        #endregion Properties
    }
}
