using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using System.Threading;
using System.Linq;
using UnityEngine;
using Runtime.Helper;

namespace Runtime.Gameplay.EntitySystem
{
    public class SummonSkillStrategy : SkillStrategy<SummonSkillModel>
    {
        protected override void Init(SummonSkillModel skillModel)
        {
            base.Init(skillModel);

        }

        protected async override UniTask PresentSkillAsync(CancellationToken cancellationToken, int index)
        {
            var spawningEntities = true;

            entityTriggerActionEventProxy.TriggerEvent(index.GetUseSkillByIndex(), callbackData =>
            {
                var suitablePosition = callbackData.spawnVFXPoints == null ? (Vector3)creatorData.Position : callbackData.spawnVFXPoints.Select(x => x.position).ToList().GetSuitableValue(creatorData.Position);
                SpawningAsync(suitablePosition, cancellationToken).Forget();
            }, data => spawningEntities = false);

            await UniTask.WaitUntil(() => !spawningEntities, cancellationToken: cancellationToken);
        }

        private async UniTaskVoid SpawningAsync(Vector2 creatorUsedToSpawn, CancellationToken token)
        {
            var spawnedCenterPosition = creatorData.Position;
            var spawnedCenterOffsetDistance = ownerModel.SummonedCenterOffsetDistance;
            var spawnedEntityInfo = ownerModel.SummonedSpawnEntitiesInfo;
            if (ownerModel.UseOwnerLevel)
            {
                spawnedEntityInfo = spawnedEntityInfo.Select(x => new SpawnedEntityInfo(x.entityId, x.entityType, creatorData.Level, x.entityNumber)).ToArray();
            }

            var spawnedPositions = await EntitiesManager.Instance.CreateEntitiesAsync(spawnedCenterPosition, spawnedCenterOffsetDistance, true, false,
                                                         token, spawnedEntityInfo);

        }
    }
}