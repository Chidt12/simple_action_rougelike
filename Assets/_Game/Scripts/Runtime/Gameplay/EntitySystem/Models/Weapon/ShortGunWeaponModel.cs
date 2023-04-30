using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShortGunWeaponModel : WeaponModel
    {
        public override WeaponType WeaponType => WeaponType.ShortGun;
        public string ProjectileId { get; private set; }
        public float ProjectileSpeed { get; private set; }

        public ShortGunWeaponModel(WeaponData weaponData) : base(weaponData)
        {
            var dataConfig = weaponData.weaponConfigItem as ShortGunWeaponDataConfigItem;
            ProjectileId = dataConfig.projectileId;
            ProjectileSpeed = dataConfig.projectileSpeed;
        }
    }
}
