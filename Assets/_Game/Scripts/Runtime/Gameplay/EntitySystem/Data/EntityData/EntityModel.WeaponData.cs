namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityWeaponData
    {
        public WeaponModel _weaponModel;
        public WeaponModel WeaponModel => _weaponModel;

        public bool IsAttacking { get; set; }

        public bool CheckCanAttack()
        {
            return !IsAttacking;
        }

        public void InitWeapon(WeaponModel weaponModel)
        {
            _weaponModel = weaponModel;
        }
    }

}