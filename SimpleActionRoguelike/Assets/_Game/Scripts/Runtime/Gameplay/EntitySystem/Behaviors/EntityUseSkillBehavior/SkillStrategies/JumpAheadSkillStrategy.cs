using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Helper;
using Runtime.Manager.Gameplay;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class JumpAheadSkillStrategy : SkillStrategy<JumpAheadSkillModel>
    {
        private bool _isJumping;

        public override bool CheckCanUseSkill()
        {
            if (ownerModel.DependTarget)
            {
                if (creatorData.Target != null && !creatorData.Target.IsDead)
                {
                    var distance = Vector2.Distance(creatorData.Position, creatorData.Target.Position);
                    if(distance <= ownerModel.JumpDistance)
                    {
                        var direction = creatorData.Target.Position - creatorData.Position;
                        var rayCastChecks = Physics2D.RaycastAll(creatorData.Position, direction, distance);

                        foreach (var rayCastCheck in rayCastChecks)
                        {
                            if(rayCastCheck.collider.gameObject.layer == Layers.OBJECT_LAYER)
                                return false;
                        }
                    }
                }
                return false;
            }
            return true;
        }


        protected async override UniTask PresentSkillAsync(CancellationToken cancellationToken, int index)
        {
            var direction = creatorData.FaceDirection;
            if (ownerModel.DependTarget)
                direction = creatorData.Target.Position - creatorData.Position;

            _isJumping = true;
            entityTriggerActionEventProxy.TriggerEvent(
                    index.GetUseSkillByIndex(),
                    stateAction: callbackData =>
                    {
                        JumpAhead(direction, cancellationToken).Forget();
                    }
                );

            await UniTask.WaitUntil(() => !_isJumping, cancellationToken: cancellationToken);
        }

        private async UniTaskVoid JumpAhead(Vector2 jumpDirection, CancellationToken token)
        {
            // trigger the farthest can go.
            var rayCastChecks = Physics2D.RaycastAll(creatorData.Position, jumpDirection, ownerModel.JumpDistance);
            var availableDistance = ownerModel.JumpDistance;

            foreach (var rayCastCheck in rayCastChecks)
            {
                var hitPoint = rayCastCheck.collider.ClosestPoint(creatorData.Position);
                var distance = Vector2.Distance(creatorData.Position, hitPoint);
                if (distance < availableDistance)
                {
                    availableDistance = distance;
                }
            }

            var predictJumpPosition = creatorData.Position + jumpDirection * availableDistance;

            float currentTime = 0;
            var jumpDuration = ownerModel.JumpDuration * availableDistance / ownerModel.JumpDistance;
            var originPosition = creatorData.Position;

            while(currentTime < jumpDuration)
            {
                currentTime += Time.deltaTime;

                var easeValue = Easing.EaseInQuad(0.0f, 1.0f, Mathf.Clamp01(currentTime / jumpDuration));
                float interpolationValue = Mathf.Lerp(0, availableDistance, easeValue);
                Vector2 moveToPosition = originPosition + jumpDirection * interpolationValue;

                if (MapManager.Instance.IsWalkable(moveToPosition))
                {
                    creatorData.ForceUpdatePosition(moveToPosition);
                }
                else
                {
                    break;
                }
                await UniTask.Yield(token);
            }

            _isJumping = false;
        }

        protected override void CancelSkill()
        {
            base.CancelSkill();
        }
    }
}