using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShortGunWeaponModel : WeaponModel
    {
        public override WeaponType WeaponType => WeaponType.ShortGun;

        public ShortGunWeaponModel(WeaponData weaponData) : base(weaponData)
        {
        }
    }
}
