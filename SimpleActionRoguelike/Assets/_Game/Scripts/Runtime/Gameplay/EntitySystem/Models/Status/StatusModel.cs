using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class StatusModel
    {
        public abstract StatusType StatusType { get; }
        protected float chance;
        public int DataId { get; protected set; }
        public abstract bool IsStackable { get; }
        public virtual int MaxStack { get; } = 0;
        public float Duration { get; protected set; }
        public bool IsAffectable => UnityEngine.Random.Range(0.0f, 1.0f) <= chance;
        public virtual void SetDuration(float duration) => Duration = duration;

        public StatusModel(StatusDataConfigItem statusDataConfig)
        {
            Duration = statusDataConfig.duration;
            chance = statusDataConfig.chance;
            DataId = statusDataConfig.dataId;
        }
    }
}