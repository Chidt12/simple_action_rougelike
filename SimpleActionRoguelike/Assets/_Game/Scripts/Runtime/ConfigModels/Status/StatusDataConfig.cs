using Runtime.Definition;
using System;
using System.Linq;
using UnityEngine;

namespace Runtime.ConfigModel
{
    [Serializable]
    public struct StatusIdentity
    {
        public int statusDataId;
        public StatusType statusType;

        public StatusIdentity(int dataId, StatusType statusType)
        {
            this.statusDataId = dataId;
            this.statusType = statusType;
        }
    }

    public abstract class StatusDataConfigItem
    {
        public int dataId;
        public float chance;
        public float duration;       
    }

    public class StatusDataConfig : ScriptableObject
    {
        public virtual StatusDataConfigItem[] Items { get; }
    }

    public abstract class StatusDataConfig<T> : StatusDataConfig where T : StatusDataConfigItem
    {
        #region Members

        public T[] items;

        #endregion Members

        #region Properties

        public override StatusDataConfigItem[] Items
            => items.Cast<StatusDataConfigItem>().ToArray();

        #endregion Properties
    }
}