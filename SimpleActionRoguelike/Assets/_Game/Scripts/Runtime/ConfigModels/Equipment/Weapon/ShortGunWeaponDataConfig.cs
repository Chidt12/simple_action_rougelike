using Runtime.Definition;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class ShortGunEquipmentMechanicDataConfigItem : EquipmentMechanicDataConfigItem { }

    [Serializable]
    public class ShortGunWeaponDataConfigItem : WeaponDataConfigItem<ShortGunEquipmentMechanicDataConfigItem>
    {
        public string projectileId;
        public float projectileSpeed;
        public override WeaponType WeaponType => WeaponType.ShortGun;
    }

    public class ShortGunWeaponDataConfig : WeaponDataConfig<ShortGunWeaponDataConfigItem, ShortGunEquipmentMechanicDataConfigItem>
    {
        public override WeaponType WeaponType => WeaponType.ShortGun;

        protected override ShortGunEquipmentMechanicDataConfigItem Add(ShortGunEquipmentMechanicDataConfigItem item1, ShortGunEquipmentMechanicDataConfigItem item2)
            => item1;
    }
}
