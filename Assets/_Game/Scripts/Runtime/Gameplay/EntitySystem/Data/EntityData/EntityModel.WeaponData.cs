namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityWeaponData
    {
        public WeaponModel _weaponModel;
        public WeaponModel WeaponModel => _weaponModel; 

        public void InitWeapon(WeaponModel weaponModel)
        {
            _weaponModel = weaponModel;
        }
    }

}