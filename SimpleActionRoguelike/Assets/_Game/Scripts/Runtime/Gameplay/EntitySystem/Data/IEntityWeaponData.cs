using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityWeaponData : IEntityData
    {
        public bool CheckCanAttack();
        public bool IsAttacking { get; set; }
        WeaponModel WeaponModel { get; }
        public void InitWeapon(WeaponModel weaponModel);
        public Action UpdateWeaponModel { get; set; }
    }
}