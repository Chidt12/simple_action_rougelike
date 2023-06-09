using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class WeaponModel 
    {
        public abstract WeaponType WeaponType { get; }
        public string WeaponPrefabName { get; protected set; }
        public int BalancingPoint { get; protected set; }

        public WeaponModel(WeaponData weaponData)
        {
            WeaponPrefabName = weaponData.weaponConfigItem.WeaponPrefabName;
            BalancingPoint = weaponData.mechanicDataConfigItem.point;
        }
    }

    public class WeaponData
    {
        public WeaponDataConfigItem weaponConfigItem;
        public EquipmentMechanicDataConfigItem mechanicDataConfigItem;

        public WeaponData(WeaponDataConfigItem weaponConfigItem, EquipmentMechanicDataConfigItem mechanicDataConfigItem)
        {
            this.weaponConfigItem = weaponConfigItem;
            this.mechanicDataConfigItem = mechanicDataConfigItem;
        }
    }
}