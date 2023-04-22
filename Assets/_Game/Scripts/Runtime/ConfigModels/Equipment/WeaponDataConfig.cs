using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System.Linq;
using UnityEngine;

namespace Runtime.ConfigModel
{
    public interface IWeaponDataConfigItem
    {
        WeaponType WeaponType { get; }
        float AttackSpeedPercent { get; }
        float DamageBonus { get; }
        DamageFactor[] DamageFactors { get; }
        EquipmentMechanicDataConfigItem[] Mechanics { get; }
    }

    public abstract class WeaponDataConfigItem<T> : IWeaponDataConfigItem where T : EquipmentMechanicDataConfigItem, new()
    {
        #region Members

        public float attackSpeedPercent;
        public float attackRange;
        public float damageBonus;
        public DamageFactor[] damageFactors;
        public T[] mechanics;

        #endregion Members

        #region Properties
        public float AttackSpeedPercent => attackSpeedPercent;

        public float AttackRange => attackRange;

        public float DamageBonus => damageBonus;

        public DamageFactor[] DamageFactors => damageFactors;

        public EquipmentMechanicDataConfigItem[] Mechanics => mechanics;

        public abstract WeaponType WeaponType { get; }

        #endregion Properties
    }

    public interface IWeaponDataConfig : IEquipmentMechanicDataConfig
    {
        #region Interface Methods

        IWeaponDataConfigItem GetWeaponDataConfigItem();

        #endregion Interface Methods
    }

    public abstract class WeaponDataConfig<T, TMechanic> : ScriptableObject, IWeaponDataConfig where T : WeaponDataConfigItem<TMechanic>
                                                                                                                             where TMechanic : EquipmentMechanicDataConfigItem, new()
    {
        #region Members

        public T[] items;

        #endregion Members

        #region Properties

        public abstract WeaponType WeaponType { get; }

        #endregion Properties

        #region Interface Methods

        public IWeaponDataConfigItem GetWeaponDataConfigItem() => items.FirstOrDefault();

        public EquipmentMechanicDataConfigItem GetEquipmentDataConfigItem(RarityType rarityType)
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

        #endregion Interface Methods
    }
}