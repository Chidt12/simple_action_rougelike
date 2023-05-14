using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityWeaponData
    {
        public WeaponModel _weaponModel;
        public WeaponModel WeaponModel => _weaponModel;

        public bool IsAttacking { get; set; }
        public Action UpdateWeaponModel { get; set; }

        public bool CheckCanAttack()
        {
            return !(IsDead || IsAttacking || IsPlayingSkill || IsDashing || currentState.IsInAttackLockedStatus());
        }

        public void InitWeapon(WeaponModel weaponModel)
        {
            _weaponModel = weaponModel;
            UpdateWeaponModel?.Invoke();
        }
    }

}