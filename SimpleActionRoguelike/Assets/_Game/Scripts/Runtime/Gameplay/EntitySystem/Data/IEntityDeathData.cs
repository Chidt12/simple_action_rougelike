using Runtime.ConfigModel;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityDeathData : IEntityData
    {
        public DeathDataIdentity DeathDataIdentity { get; }
    }
}