namespace Runtime.Gameplay.EntitySystem
{
    public interface IUpdateHealthModifier
    {
        int UpdateHealthPriority { get; }

        #region Interface Methods

        (float, bool) ModifyBuffHp(float value, bool isCrit);

        float ModifyDebuffHp(float value, bool isCrit, IEntityData creatorData);

        #endregion Interface Methods
    }
}