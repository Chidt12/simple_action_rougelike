using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Pool;
using Runtime.Definition;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class StatusAroundAreaArtifactSystem : RuneArtifactSystem<StatusArroundAreaArtifactDataConfigItem>
    {
        public override ArtifactType ArtifactType => ArtifactType.StatusAroundArea;


        public override void Trigger()
        {
            base.Trigger();

            SpawnDamageBoxAsync(cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid SpawnDamageBoxAsync(CancellationToken cancellationToken)
        {
            var damageBoxGameObject = await PoolManager.Instance.Rent(ownerData.damageBoxPrefabName, token: cancellationToken);
            var damageBox = damageBoxGameObject.GetComponent<AnimatorDamageBox>();
            damageBox.Init(ownerEntityData, EffectSource.FromArtifact, EffectProperty.Normal, ownerData.damageBonus, ownerData.damageFactors, ownerData.triggeredStatus);
            damageBox.Scale(new Vector2(ownerData.damageBoxWidth / 2, ownerData.damageBoxHeight / 2));
            var position = ownerEntityData.Position;

            if(ownerData.statusAroundSpawnPosition == StatusAroundSpawnPositionType.TowardDirection)
            {
                var direction = ((IEntityControlData)ownerEntityData).FaceDirection;
                damageBox.transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);
            }
            else if (ownerData.statusAroundSpawnPosition == StatusAroundSpawnPositionType.InPointerPoint)
            {
                position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            damageBox.transform.position = position;
        }
    }
}