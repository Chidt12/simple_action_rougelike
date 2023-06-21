namespace Runtime.Gameplay.EntitySystem
{
    public interface IFinalDamagedModifier
    {
        public void Finalize(float damageCreated, IEntityData receiver);
    }
}