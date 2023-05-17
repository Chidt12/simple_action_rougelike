using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System.Linq;

namespace Runtime.ConfigModel
{
    public interface IWeaponDataConfigItem
    {
        WeaponType WeaponType { get; }
        float AttackSpeedPercent { get; }
        float AttackRange { get; }
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

    public abstract class WeaponDataConfig : EquipmentMechanicDataConfig
    {
        #region Interface Methods

        public abstract IWeaponDataConfigItem GetWeaponDataConfigItem();

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

        public override IWeaponDataConfigItem GetWeaponDataConfigItem() => items.FirstOrDefault();

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
            var itemData = GetWeaponDataConfigItem() as T;
            return GetDescription(rarityType, itemData, mechanicData);
        }

        protected abstract UniTask<string> GetDescription(RarityType rarityType, T itemData, TMechanic mechanicData);

        #endregion Interface Methods
    }
}