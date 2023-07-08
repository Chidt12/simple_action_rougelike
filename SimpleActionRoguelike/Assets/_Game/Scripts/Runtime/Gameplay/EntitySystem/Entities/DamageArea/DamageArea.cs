using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    /// <summary>
    /// Damage area is scalable, cause damage over time like trap, but spawned by characters.
    /// </summary>
    public class DamageAreaData
    {

        public IEntityData creatorData;
        public EffectSource damageSource;
        public EffectProperty damageProperty;
        public float lifeTime;
        public float damageInterval;
        public float width;
        public float height;
        public DamageFactor[] damageAreaDamageFactors;
        public DamageFactor[] firstInitDamageFactors;

        public DamageAreaData(IEntityData creatorData, float lifeTime, float damageInterval, EffectSource damageSource, EffectProperty damageProperty,
                              float width, float height, DamageFactor[] damageAreaDamageFactors,
                              DamageFactor[] firstInitDamageFactors)
        {
            this.creatorData = creatorData;
            this.damageSource = damageSource;
            this.damageProperty = damageProperty;
            this.damageAreaDamageFactors = damageAreaDamageFactors;
            this.lifeTime = lifeTime;
            this.width = width;
            this.height = height;
            this.damageInterval = damageInterval;
            this.firstInitDamageFactors = firstInitDamageFactors;
        }
    }

    public class DamageArea : Disposable
    {
        protected float currentDamageTime;
        protected float currentLifetime;
        protected List<IEntityData> damagedTargets;
        protected DamageAreaData data;

        private void Update()
        {
            if (currentDamageTime > data.damageInterval)
            {
                currentDamageTime = 0.0f;
                DamageTargets();
            }
            else currentDamageTime += Time.deltaTime;

            if (currentLifetime <= data.lifeTime)
                currentLifetime += Time.deltaTime;
            else
                DestroySelf();
        }

        private void OnDisable() => Dispose();

        public virtual async UniTask BuildAsync(IEntityData creatorData, Vector3 position, DamageAreaData data)
        {
            transform.position = position;
            transform.localScale = new Vector2(data.width / 2, data.height / 2);
            this.data = data;
            currentLifetime = 0;
            damagedTargets = new();

            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }

        private void DamageTargets()
        {
            foreach (var damagedTarget in damagedTargets)
            {
                if (data.creatorData.EntityType.CanCauseDamage(damagedTarget.EntityType))
                {
                    SimpleMessenger.PublishAsync(MessageScope.EntityMessage,
                        new SentDamageMessage(data.damageSource, data.damageProperty,
                        0, data.damageAreaDamageFactors, data.creatorData, damagedTarget)).Forget();
                }
            }
        }

        private void DestroySelf() => PoolManager.Instance.Return(gameObject);

        public override void Dispose()
        {
        }
    }
}
