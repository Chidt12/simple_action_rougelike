using UnityEngine;

namespace Runtime.ConfigModel
{
    public class BaseConfig<TItem> : ScriptableObject
    {
        #region Members

        public TItem[] items;

        #endregion Members
    }
}