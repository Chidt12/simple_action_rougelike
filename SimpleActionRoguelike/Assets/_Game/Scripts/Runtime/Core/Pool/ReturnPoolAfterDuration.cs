using UnityEngine;

namespace Runtime.Core.Pool
{
    public class ReturnPoolAfterDuration : ReturnPool
    {
        [SerializeField]
        [Min(0.001f)]
        protected float duration = 1.0f;

        protected override float ReturnDelayTime => duration;
    }
}