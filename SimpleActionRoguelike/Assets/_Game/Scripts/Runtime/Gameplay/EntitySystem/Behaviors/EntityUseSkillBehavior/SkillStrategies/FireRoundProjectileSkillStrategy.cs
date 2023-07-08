using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FireRoundProjectileSkillStrategy : SkillStrategy<FireRoundProjectileSkillModel>
    {
        public override bool CheckCanUseSkill()
        {
            if (ownerModel.DependTarget)
            {
                if (creatorData.Target != null && !creatorData.Target.IsDead)
                {
                    var distance = Vector2.Distance(creatorData.Position, creatorData.Target.Position);
                    return distance <= ownerModel.MaxProjectileFlyDistance;
                }
                return false;
            }
            return true;
        }


        protected async override UniTask PresentSkillAsync(CancellationToken cancellationToken, int index)
        {
            //for (int i = 0; i < ownerModel.NumberOfProjectiles; i++)
            //{
            //    var hasFinishedAnimation = false;
            //    entityTriggerActionEventProxy.TriggerEvent(
            //        index.GetUseSkillByIndex(),
            //        stateAction: callbackData =>
            //        {
            //            var suitablePosition = callbackData.spawnVFXPoints == null ? (Vector3)creatorData.Position : callbackData.spawnVFXPoints.Select(x => x.position).ToList().GetSuitableValue(creatorData.Position);
            //            FireProjectile(suitablePosition, direction, cancellationToken).Forget();
            //        },
            //        endAction: callbackData => hasFinishedAnimation = true
            //    );

            //    await UniTask.WaitUntil(() => hasFinishedAnimation, cancellationToken: cancellationToken);
            //    if (ownerModel.DelayBetweenProjectile > 0)
            //        await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.DelayBetweenProjectile), cancellationToken: cancellationToken);
            //}


            
        }
    }
}