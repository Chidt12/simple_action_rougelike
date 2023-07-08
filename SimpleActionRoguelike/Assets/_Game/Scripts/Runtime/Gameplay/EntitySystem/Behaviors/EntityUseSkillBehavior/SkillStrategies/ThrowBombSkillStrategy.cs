using Cysharp.Threading.Tasks;
using Pathfinding;
using Runtime.Core.Pool;
using Runtime.Manager.Gameplay;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Gameplay.EntitySystem
{
    public class ThrowBombSkillStrategy : SkillStrategy<ThrowBombSkillModel>
    {
        public override bool CheckCanUseSkill()
        {
            if (ownerModel.DependTarget)
            {
                if (creatorData.Target != null && !creatorData.Target.IsDead)
                {
                    return true;
                }
                return false;
            }
            return true;
        }


        protected async override UniTask PresentSkillAsync(CancellationToken cancellationToken, int index)
        {
            entityTriggerActionEventProxy.TriggerEvent(index.GetUseSkillByIndex());

            for (int i = 0; i < ownerModel.NumberOfBombs; i++)
            {

                var spawningPositions = new List<GridNode>();
                var centerPoint = ownerModel.DependTarget ? creatorData.Target.Position : creatorData.Position;
                var spawnPosition = centerPoint;
                if (ownerModel.Random)
                {
                    spawningPositions = MapManager.Instance.GetAllWalkableNodesInRange(centerPoint, ownerModel.OffsetRandom);
                    spawnPosition = (Vector3)spawningPositions[Random.Range(0, spawningPositions.Count)].position;
                }

                SpawnBombAsync(spawnPosition).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.DelayBetweenBombs), cancellationToken: cancellationToken);
            }
        }

        private async UniTaskVoid SpawnBombAsync(Vector2 spawnPosition)
        {
            var warningObject = await PoolManager.Instance.Rent(ownerModel.WarningPrefabName);
            var warning = warningObject.GetComponent<WarningDamageVFX>();
            warning.Init(spawnPosition, new Vector2(ownerModel.ImpactWidth / 2, ownerModel.ImpactHeight / 2));
            await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.WarningTime));
            var impactObject = await PoolManager.Instance.Rent(ownerModel.ImpactPrefabName);
            var impact = impactObject.GetComponent<AnimatorDamageBox>();
            impactObject.transform.position = spawnPosition;
            impact.Init(creatorData, EffectSource.FromSkill, EffectProperty.Normal, ownerModel.DamageBonus, ownerModel.DamageFactors, default, onTurnOn: () => {
                if (warningObject)
                {
                    PoolManager.Instance.Return(warningObject);
                }
            });
        }

    }
}