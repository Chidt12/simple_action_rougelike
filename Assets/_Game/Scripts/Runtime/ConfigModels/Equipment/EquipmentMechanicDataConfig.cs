using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.ConfigModel
{
    [Serializable]
    public abstract class EquipmentMechanicDataConfigItem
    {
        #region Members

        public RarityType triggerRarityType;

        #endregion Members
    }

    public abstract class EquipmentMechanicDataConfig : ScriptableObject
    {
        #region Interface Methods

        public abstract EquipmentMechanicDataConfigItem GetEquipmentDataConfigItem(RarityType rarityType);

        #endregion Interface Methods
    }

    public abstract class EquipmentMechanicDataConfig<T> : EquipmentMechanicDataConfig where T : EquipmentMechanicDataConfigItem, new()
    {
        #region Members

        public T[] items;

        #endregion Members

        #region Properties

        public abstract EquipmentSystemType EquipmentSystemType { get; }

        #endregion Properties

        #region Class Methods

        public override EquipmentMechanicDataConfigItem GetEquipmentDataConfigItem(RarityType rarityType)
        {
            var equipmentDataConfigItem = new T();
            foreach (var item in items)
            {
                if (item.triggerRarityType <= rarityType)
                    Add(equipmentDataConfigItem, item);
            }
            return equipmentDataConfigItem;
        }

        protected abstract T Add(T item1, T item2);

        #endregion Class Methods
    }
}