using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShortGunWeaponModel : WeaponModel
    {
        public override WeaponType WeaponType => WeaponType.ShortGun;
        public string ProjectileId { get; private set; }
        public float ProjectileSpeed { get; private set; }
        public int NumberOfProjectilesInVertical { get; private set; }
        public int NumberOfProjectilesInHorizontal { get; private set; }
        public bool GoThrough { get; private set; }

        public ShortGunWeaponModel(WeaponData weaponData) : base(weaponData)
        {
            var dataConfig = weaponData.weaponConfigItem as ShortGunWeaponDataConfigItem;
            var mechanicDataConfig = weaponData.mechanicDataConfigItem as ShortGunEquipmentMechanicDataConfigItem;
            ProjectileId = dataConfig.projectileId;
            ProjectileSpeed = dataConfig.projectileSpeed;

            NumberOfProjectilesInHorizontal = 1 + mechanicDataConfig.bonusProjectileHorizontal;
            NumberOfProjectilesInVertical = 1 + mechanicDataConfig.bonusProjectileVertical;
            GoThrough = mechanicDataConfig.goThrough;
        }
    }
}
