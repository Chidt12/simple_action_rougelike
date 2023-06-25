using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class StatusStormArtifactSystem : RuneArtifactSystem<StatusStormArtifactDataConfigItem>
    {
        public override ArtifactType ArtifactType => ArtifactType.StatusStorm;
        private CancellationTokenSource _cancellationTokenSource;

        public override void Dispose()
        {
            base.Dispose();
            _cancellationTokenSource.Cancel();
        }

        public override void Trigger()
        {
            _cancellationTokenSource = new();
            SpawnLightningAsync(_cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid SpawnLightningAsync(CancellationToken cancellationToken)
        {
            var allPoints = MapManager.Instance.ActiveGraph.nodes.Where(x => x.Walkable 
                                && Vector2.Distance(ownerEntityData.Position, (Vector3)x.position) <= ownerData.range).ToList();

            for (int i = 0; i < ownerData.numberOfLightning; i++)
            {
                var lightningObject = await PoolManager.Instance.Rent(ownerData.ligntningPrefab, token: cancellationToken);
                lightningObject.transform.position = (Vector3)allPoints[UnityEngine.Random.Range(0, allPoints.Count)].position;
                var damageBox = lightningObject.GetComponent<AnimatorDamageBox>();
                damageBox.Init(OnEntityEntered);
                await UniTask.Delay(TimeSpan.FromSeconds(ownerData.delayBetweenLightning), cancellationToken: cancellationToken);
            }
        }

        private void OnEntityEntered(IEntityData obj)
        {
            if (obj.EntityType.IsEnemy())
            {
                SimpleMessenger.PublishAsync(MessageScope.EntityMessage,
                    new SentDamageMessage(EffectSource.FromArtifact, EffectProperty.Normal, 
                    ownerData.damageBonus, ownerData.damageFactors, ownerEntityData, obj)).Forget();

                var targetStatusData = obj as IEntityStatusData;
                SimpleMessenger.PublishAsync(MessageScope.EntityMessage,
                    new SentStatusEffectMessage(ownerEntityData, targetStatusData, ownerData.triggeredStatus)).Forget();
            }
        }
    }
}