namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityWeaponData : IEntityData
    {
        public bool CheckCanAttack();
        public bool IsAttacking { get; set; }
        WeaponModel WeaponModel { get; }
    }
}