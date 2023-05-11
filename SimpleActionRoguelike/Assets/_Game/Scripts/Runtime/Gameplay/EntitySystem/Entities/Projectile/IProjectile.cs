using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IProjectile
    {
        IEntityData Creator { get; }
        UniTask BuildAsync(IEntityData creatorData, Vector3 position);
        void InitStrategy(IProjectileStrategy projectileStrategy);
        void CompleteStrategy(bool displayImpact);

        /// <summary>
        /// Use to adjust speed of projectile
        /// </summary>
        /// <param name="adjustSpeedFactor"></param>
        void UpdateAdjustSpeedFactor(float adjustSpeedFactor);
    }
}