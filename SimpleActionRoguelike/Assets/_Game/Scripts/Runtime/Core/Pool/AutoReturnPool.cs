using UnityEngine;

namespace Runtime.Core.Pool
{
    public class AutoReturnPool : ReturnPool
    {
        [SerializeField]
        [Tooltip("If this is disabled, then the return delay time determines the end time.")]
        protected bool isReturnByParticleTime;

        [SerializeField]
        [Min(0.001f)]
        protected float returnDelayTime;

        protected override float ReturnDelayTime
        {
            get
            {
                if (isReturnByParticleTime)
                {
                    float particleTime = returnDelayTime;
                    var particleSystems = transform.GetComponentsInChildren<ParticleSystem>();

                    foreach (ParticleSystem particleSystem in particleSystems)
                        if (particleSystem.main.startLifetime.constantMax > particleTime)
                            particleTime = particleSystem.main.startLifetime.constantMax * (1 / particleSystem.main.simulationSpeed);

                    return particleTime;
                }

                return returnDelayTime;
            }
        }
    }
}
