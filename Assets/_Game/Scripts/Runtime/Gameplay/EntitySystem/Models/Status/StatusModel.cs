using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class StatusModel
    {
        protected float chance;
        public abstract bool IsStackable { get; }
        public abstract int MaxStack { get; }
        public float Duration { get; protected set; }
        public bool IsAffectable => UnityEngine.Random.Range(0.0f, 1.0f) <= chance;
        public virtual void SetDuration(float duration) => Duration = duration;
        public virtual void Stack(StatusModel stackedStatusModel, bool isMaxStack) { }
    }
}