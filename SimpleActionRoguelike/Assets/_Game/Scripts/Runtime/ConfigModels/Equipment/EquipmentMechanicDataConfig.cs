using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.ConfigModel
{
    [Serializable]
    public abstract class EquipmentMechanicDataConfigItem : BaseWithPointConfigItem
    {
        public RarityType triggerRarityType;
    }

    public abstract class EquipmentMechanicDataConfig : ScriptableObject
    {

        public abstract EquipmentMechanicDataConfigItem GetEquipmentMechanicDataConfigItem(RarityType rarityType);
        public abstract UniTask<string> GetDescription(RarityType rarityType);
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

        public override EquipmentMechanicDataConfigItem GetEquipmentMechanicDataConfigItem(RarityType rarityType)
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

        public override UniTask<string> GetDescription(RarityType rarityType)
        {
            var configItem = GetEquipmentMechanicDataConfigItem(rarityType) as T;
            return GetDescription(configItem);
        }

        protected abstract UniTask<string> GetDescription(T itemData);

        #endregion Class Methods
    }
}