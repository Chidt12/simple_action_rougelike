using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BoomerangWeaponModel : WeaponModel
    {
        public override WeaponType WeaponType => WeaponType.Boomerang;
        public string ProjectileId { get; private set; }
        public float ProjectileSpeed { get; private set; }
        public int NumberOfProjectiles { get; private set; }
        public bool GoThrough { get; private set; }
        public StatusIdentity StatusIdentity { get; private set; }
        public bool CanSendStatus => StatusIdentity.statusType != StatusType.None;

        public BoomerangWeaponModel(WeaponData weaponData) : base(weaponData)
        {
            var dataConfig = weaponData.weaponConfigItem as BoomerangWeaponDataConfigItem;
            var mechanicDataConfig = weaponData.mechanicDataConfigItem as BoomerangEquipmentMechanicDataConfigItem;

            ProjectileId = dataConfig.projectileId;
            ProjectileSpeed = dataConfig.projectileSpeed + mechanicDataConfig.buffFlySpeed;
            NumberOfProjectiles = 1 + mechanicDataConfig.bonusProjectiles;
            GoThrough = mechanicDataConfig.goThrough;
            StatusIdentity = mechanicDataConfig.buffStatusIdentity;
        }
    }
}