using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Message;
using System;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public class StatusForwardAreaArtifactSystem : RuneArtifactSystem<StatusForwardAreaArtifactDataConfigItem>
    {
        public override ArtifactType ArtifactType => ArtifactType.StatusForwardArea;

        private CancellationTokenSource _cancellationTokenSource;

        public override void Dispose()
        {
            _cancellationTokenSource.Cancel();
            base.Dispose();
        }

        public override void Trigger()
        {
            _cancellationTokenSource = new();
            SpawnForwardObjectsAsync(_cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid SpawnForwardObjectsAsync(CancellationToken cancellationToken)
        {
            var entityData = ownerEntityData as IEntityControlData;
            if (entityData != null)
            {
                var direction = entityData.FaceDirection;
                var position = ownerEntityData.Position;
                for (int i = 0; i < ownerData.numberOfObjects; i++)
                {
                    var spawnForwardObject = await PoolManager.Instance.Rent(ownerData.forwardPrefabName, token: cancellationToken);
                    spawnForwardObject.transform.position = position + direction.normalized * ownerData.distanceBetweenObject * (i+1);
                    var damageBox = spawnForwardObject.GetComponent<AnimatorDamageBox>();
                    damageBox.Init(OnTriggeredEntered);
                    await UniTask.Delay(TimeSpan.FromSeconds(ownerData.delayBetweenSpawn), cancellationToken: cancellationToken);
                }
            }
        }

        private void OnTriggeredEntered(IEntityData entityData)
        {
            if (entityData.EntityType.IsEnemy())
            {
                SimpleMessenger.PublishAsync(MessageScope.EntityMessage,
                    new SentDamageMessage(EffectSource.FromArtifact, EffectProperty.Normal,
                    ownerData.damageBonus, ownerData.damageFactors, ownerEntityData, entityData)).Forget();

                var targetStatusData = entityData as IEntityStatusData;
                SimpleMessenger.PublishAsync(MessageScope.EntityMessage,
                    new SentStatusEffectMessage(ownerEntityData, targetStatusData, ownerData.triggeredStatus)).Forget();
            }
        }
    }
}