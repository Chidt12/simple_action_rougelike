using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System.Linq;

namespace Runtime.ConfigModel
{
    public abstract class WeaponDataConfigItem
    {
        // For Buff Stat Hero
        public EquipmentStat[] stats;

        public abstract string WeaponPrefabName { get; }
        public abstract WeaponType WeaponType { get; }
        public abstract EquipmentMechanicDataConfigItem[] Mechanics { get; }
    }

    public abstract class WeaponDataConfigItem<T> : WeaponDataConfigItem where T : EquipmentMechanicDataConfigItem, new()
    {
        public string weaponPrefabName;
        public T[] mechanics;

        public override string WeaponPrefabName => weaponPrefabName;
        public override EquipmentMechanicDataConfigItem[] Mechanics => mechanics;
    }

    public abstract class WeaponDataConfig : EquipmentMechanicDataConfig
    {
        #region Interface Methods

        public abstract WeaponDataConfigItem GetWeaponDataConfigItem();

        #endregion Interface Methods
    }

    public abstract class WeaponDataConfig<T, TMechanic> : WeaponDataConfig where T : WeaponDataConfigItem<TMechanic>
        where TMechanic : EquipmentMechanicDataConfigItem, new()
    {
        #region Members

        public T[] items;

        #endregion Members

        #region Properties

        public abstract WeaponType WeaponType { get; }

        #endregion Properties

        #region Interface Methods

        public override WeaponDataConfigItem GetWeaponDataConfigItem() => items.FirstOrDefault();

        public override EquipmentMechanicDataConfigItem GetEquipmentMechanicDataConfigItem(RarityType rarityType)
        {
            var weaponConfig = GetWeaponDataConfigItem();
            if (weaponConfig != null && weaponConfig.Mechanics != null)
            {
                var equipmentDataConfigItem = new TMechanic();
                var mechanicItems = weaponConfig.Mechanics;
                foreach (var item in mechanicItems)
                {
                    if (item.triggerRarityType <= rarityType)
                        Add(equipmentDataConfigItem, item as TMechanic);
                }
                return equipmentDataConfigItem;
            }
            return null;
        }

        protected abstract TMechanic Add(TMechanic item1, TMechanic item2);

        public override UniTask<string> GetDescription(RarityType rarityType)
        {
            var mechanicData = GetEquipmentMechanicDataConfigItem(rarityType) as TMechanic;

            TMechanic previousMechanicData = null;
            if (RarityType.Common < rarityType)
            {
                previousMechanicData = GetEquipmentMechanicDataConfigItem(rarityType - 1) as TMechanic;
            }
            var itemData = GetWeaponDataConfigItem() as T;
            return GetDescription(rarityType, itemData, mechanicData, previousMechanicData);
        }

        protected abstract UniTask<string> GetDescription(RarityType rarityType, T itemData, TMechanic mechanicData, TMechanic previousMechanicData);

        #endregion Interface Methods
    }
}