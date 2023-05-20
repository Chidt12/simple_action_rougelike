using Runtime.ConfigModel;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityDeathData
    {
        protected DeathDataIdentity deathDataIdentity;
        public DeathDataIdentity DeathDataIdentity => deathDataIdentity;

        public void InitDeathData(DeathDataIdentity deathDataIdentity)
        {
            this.deathDataIdentity = deathDataIdentity;
        }
    }

}