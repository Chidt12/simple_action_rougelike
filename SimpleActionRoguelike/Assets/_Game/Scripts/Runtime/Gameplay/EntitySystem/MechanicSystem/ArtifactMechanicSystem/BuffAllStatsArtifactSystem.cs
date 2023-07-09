using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Pool;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class BuffAllStatsArtifactSystem : RuneArtifactSystem<BuffAllStatsArtifactDataConfigItem>
    {
        public override ArtifactType ArtifactType => ArtifactType.BuffAllStats;

        private bool isBuffed;
        private float currentCountBuffTime;

        public override bool Trigger()
        {
            if (isBuffed)
            {
                currentCountBuffTime = 0;
            }
            else
            {
                isBuffed = true;
                SpawnChangePrefabNameAsync().Forget();
                ownerEntityData.ChangeFormEvent.Invoke(ownerData.formId);
                currentCountBuffTime = 0;
                var entityStatData = ownerEntityData as IEntityModifiedStatData;
                foreach (var buffStat in ownerData.buffStats)
                {
                    entityStatData.BuffStat(buffStat.statType, buffStat.value, buffStat.statModifyType);
                }
            }

            return true;
        }

        private async UniTaskVoid SpawnChangePrefabNameAsync()
        {
            var vfx = await PoolManager.Instance.Rent(ownerData.changeFormPrefab, token: cancellationTokenSource.Token);
            vfx.transform.position = ownerEntityData.Position;
        }

        protected override void OnUpdateAsync()
        {
            base.OnUpdateAsync();

            currentCountBuffTime = currentCountBuffTime + Time.deltaTime;

            if(currentCountBuffTime >= ownerData.buffDuration)
            {
                if (isBuffed)
                {
                    isBuffed = false;
                    SpawnChangePrefabNameAsync().Forget();
                    ownerEntityData.ChangeFormEvent.Invoke(0);
                    var entityStatData = ownerEntityData as IEntityModifiedStatData;
                    foreach (var buffStat in ownerData.buffStats)
                    {
                        entityStatData.DebuffStat(buffStat.statType, buffStat.value, buffStat.statModifyType);
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            ownerEntityData.ChangeFormEvent.Invoke(0);
        }
    }
}