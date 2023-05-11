using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class StunStatusModel : StatusModel
    {
        public override StatusType StatusType => StatusType.Stun;

        public override bool IsStackable => false;

        public StunStatusModel(StatusDataConfigItem statusDataConfig) : base(statusDataConfig)
        {
        }
    }
}