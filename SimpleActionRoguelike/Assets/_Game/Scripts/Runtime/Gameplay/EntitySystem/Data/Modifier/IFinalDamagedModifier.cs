namespace Runtime.Gameplay.EntitySystem
{
    public interface IFinalDamagedModifier
    {
        public float Finalize(float damageCreated, IEntityData receiver);
    }
}