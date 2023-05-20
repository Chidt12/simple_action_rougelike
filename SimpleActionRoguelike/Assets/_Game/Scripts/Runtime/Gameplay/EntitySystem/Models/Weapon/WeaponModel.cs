using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class WeaponModel 
    {
        public abstract WeaponType WeaponType { get; }
        public float DamageBonus { get; }
        public DamageFactor[] DamageFactors { get; }
        public float AttackSpeedPercent { get; protected set; }
        public float AttackRange { get; protected set; }
        public string WeaponPrefabName { get; protected set; }

        public WeaponModel(WeaponData weaponData)
        {
            WeaponPrefabName = weaponData.weaponConfigItem.WeaponPrefabName;
            AttackSpeedPercent = weaponData.weaponConfigItem.AttackSpeedPercent;
            AttackRange = weaponData.weaponConfigItem.AttackRange;
            DamageBonus = weaponData.weaponConfigItem.DamageBonus;
            DamageFactors = weaponData.weaponConfigItem.DamageFactors;
        }
    }

    public class WeaponData
    {
        public IWeaponDataConfigItem weaponConfigItem;
        public EquipmentMechanicDataConfigItem mechanicDataConfigItem;

        public WeaponData(IWeaponDataConfigItem weaponConfigItem, EquipmentMechanicDataConfigItem mechanicDataConfigItem)
        {
            this.weaponConfigItem = weaponConfigItem;
            this.mechanicDataConfigItem = mechanicDataConfigItem;
        }
    }
}