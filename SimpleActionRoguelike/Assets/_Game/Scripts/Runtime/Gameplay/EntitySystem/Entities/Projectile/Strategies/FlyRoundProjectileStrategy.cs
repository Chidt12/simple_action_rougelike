using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyRoundProjectileStrategyData : ProjectileStrategyData
    {
        public float flyDuration;
        public float flyHeight;
        public string warningPrefabName;
        public float warningHeight;
        public float warningWidth;

        public FlyRoundProjectileStrategyData(
            float flyDuration, float flyHeight, string warningPrefabName, float warningHeight, float warningWidth,
            Action<ProjectileCallbackData> callbackAction) : base(callbackAction)
        {
            this.flyDuration = flyDuration;
            this.flyHeight = flyHeight;
            this.warningPrefabName = warningPrefabName;
            this.warningHeight = warningHeight;
            this.warningWidth = warningWidth;
        }
    }

    public class FlyRoundProjectileStrategy : ProjectileStrategy<FlyRoundProjectileStrategyData>
    {
        protected Vector2 targetPosition;
        protected Vector2 originalPosition;
        protected Vector2 middlePosition;
        protected float currentFlyTime;
        protected GameObject warningGameObject;

        public override void Init(ProjectileStrategyData projectileStrategyData, Projectile controllerProjectile, Vector2 direction, Vector2 originalPosition, Vector2 targetPosition, IEntityData targetData = null)
        {
            base.Init(projectileStrategyData, controllerProjectile, direction, originalPosition, targetPosition, targetData);

            targetPosition = targetData.Position;
            this.originalPosition = originalPosition;
            middlePosition = new Vector2((targetPosition.x + originalPosition.x) / 2, (targetPosition.y + originalPosition.y) / 2 + strategyData.flyHeight);
            currentFlyTime = 0;
            if (!string.IsNullOrEmpty(strategyData.warningPrefabName))
            {
                SpawnWarningVfx(targetPosition).Forget();
            }
        }

        private async UniTaskVoid SpawnWarningVfx(Vector2 spawnPosition)
        {
            warningGameObject = await PoolManager.Instance.Rent(strategyData.warningPrefabName, token: cancellationTokenSource.Token);
            var warningDamageArea = warningGameObject.GetComponent<WarningDamageVFX>();
            warningDamageArea.Init(spawnPosition, new Vector2(strategyData.warningWidth / 2, strategyData.warningHeight / 2));
        }

        public override void Update()
        {
            base.Update();
            if (currentFlyTime >= strategyData.flyDuration)
            {
                if (warningGameObject)
                {
                    PoolManager.Instance.Return(warningGameObject);
                }

                Complete(false, true);
            }
            else
            {
                currentFlyTime += Time.deltaTime;
                var moveToPosition = Helper.Helper.Bezier(originalPosition, middlePosition, targetPosition, Mathf.Clamp01(currentFlyTime / strategyData.flyDuration));
                controllerProjectile.UpdatePosition(moveToPosition);
            }
        }
    }
}