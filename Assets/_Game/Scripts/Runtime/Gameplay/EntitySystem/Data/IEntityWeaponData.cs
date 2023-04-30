namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityWeaponData : IEntityData
    {
        WeaponModel WeaponModel { get; }
    }
}