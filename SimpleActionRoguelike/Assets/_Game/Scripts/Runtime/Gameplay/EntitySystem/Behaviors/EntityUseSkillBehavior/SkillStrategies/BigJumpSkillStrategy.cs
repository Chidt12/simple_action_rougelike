using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Message;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class BigJumpSkillStrategy : SkillStrategy<BigJumpSkillModel>
    {
        private bool _isJumping;
        private GameObject _warningVfx;

        public override bool CheckCanUseSkill()
        {
            if (ownerModel.DependTarget)
            {
                if (creatorData.Target != null && !creatorData.Target.IsDead)
                {
                    var distance = Vector2.Distance(creatorData.Position, creatorData.Target.Position);
                    return distance <= ownerModel.JumpDistance;
                }
                return false;
            }
            return true;
        }

        protected async override UniTask PresentSkillAsync(CancellationToken cancellationToken, int index)
        {
            for (int i = 0; i < ownerModel.NumberOfJump; i++)
            {
                var direction = creatorData.FaceDirection;
                if (ownerModel.DependTarget)
                    direction = creatorData.Target.Position - creatorData.Position;

                _isJumping = true;
                entityTriggerActionEventProxy.TriggerEvent(
                        index.GetUseSkillByIndex(),
                        stateAction: callbackData =>
                        {
                            Jump(direction, index, cancellationToken).Forget();
                        }
                    );

                await UniTask.WaitUntil(() => !_isJumping, cancellationToken: cancellationToken);
                if (i != ownerModel.NumberOfJump - 1)
                    await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.DelayBetweenJump), cancellationToken: cancellationToken);
            }
        }

        private async UniTaskVoid Jump(Vector2 jumpDirection, int index, CancellationToken token)
        {
            var startJumpPosition = creatorData.Position;
            var originPosition = creatorData.Position;
            float currentTime = 0;


            creatorData.IsInvincible = true;

            // Jump Up
            var heightOriginPosition = creatorData.Position + Vector2.up * ownerModel.JumpHeight;
            while (currentTime < ownerModel.JumpUpDuration)
            {
                currentTime += Time.deltaTime;
                var moveToPosition = Helper.Helper.Bezier(originPosition, heightOriginPosition, Mathf.Clamp01(currentTime / ownerModel.JumpUpDuration));
                creatorData.ForceUpdatePosition?.Invoke(moveToPosition);
                await UniTask.Yield(token);
            }

            // Jump to heigh predict position

            var predictJumpPosition = creatorData.Position;

            if (ownerModel.DependTarget && creatorData.Target != null)
            {
                currentTime = 0;
                originPosition = creatorData.Position;
                while (currentTime < ownerModel.JumpMiddleDuration)
                {
                    if (creatorData.Target == null || creatorData.Target.IsDead)
                        break;

                    currentTime += Time.deltaTime;
                    var moveToPosition = Helper.Helper.Bezier(originPosition, creatorData.Target.Position + Vector2.up * ownerModel.JumpHeight, Mathf.Clamp01(currentTime / ownerModel.JumpMiddleDuration));
                    creatorData.ForceUpdatePosition?.Invoke(moveToPosition);
                    await UniTask.Yield(token);
                }

                
                predictJumpPosition = creatorData.Target == null ? startJumpPosition : creatorData.Target.Position;
            }
            else
            {
                // trigger the farthest can go.
                var rayCastChecks = Physics2D.RaycastAll(creatorData.Position, jumpDirection, ownerModel.JumpDistance);
                var availableDistance = ownerModel.JumpDistance;
                foreach (var rayCastCheck in rayCastChecks)
                {
                    if (rayCastCheck.collider.gameObject.layer == Layers.OBJECT_LAYER)
                    {
                        var hitPoint = rayCastCheck.collider.ClosestPoint(creatorData.Position);
                        var distance = Vector2.Distance(creatorData.Position, hitPoint);
                        if (distance < availableDistance)
                            availableDistance = distance;
                    }
                }

                predictJumpPosition = creatorData.Position + jumpDirection * availableDistance;
                currentTime = 0;
                originPosition = creatorData.Position;
                var upperPredictJumpPosition = predictJumpPosition + Vector2.up * ownerModel.JumpHeight;
                while (currentTime < ownerModel.JumpMiddleDuration)
                {
                    currentTime += Time.deltaTime;
                    var moveToPosition = Helper.Helper.Bezier(originPosition, upperPredictJumpPosition, Mathf.Clamp01(currentTime / ownerModel.JumpMiddleDuration));
                    creatorData.ForceUpdatePosition?.Invoke(moveToPosition);
                    await UniTask.Yield(token);
                }
            }

            // Jump down

            WarningDamageVFX warningDamageVfx = null;
            if (!string.IsNullOrEmpty(ownerModel.WarningVfx))
            {
                _warningVfx = await PoolManager.Instance.Rent(ownerModel.WarningVfx);
                _warningVfx.transform.position = predictJumpPosition;
                warningDamageVfx = _warningVfx.GetComponent<WarningDamageVFX>();
                warningDamageVfx.Init(new Vector2(ownerModel.DamageWidth, ownerModel.DamageHeight));
                await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.DisplayWarningTime), cancellationToken: token);
            }

            currentTime = 0;
            originPosition = creatorData.Position;
            while (currentTime < ownerModel.JumpDownDuration)
            {
                currentTime += Time.deltaTime;
                var moveToPosition = Helper.Helper.Bezier(originPosition, predictJumpPosition, Mathf.Clamp01(currentTime / ownerModel.JumpDownDuration));
                creatorData.ForceUpdatePosition?.Invoke(moveToPosition);
                await UniTask.Yield(token);
            }


            entityTriggerActionEventProxy.TriggerEvent(
                    index.GetUseSkillPartByIndex(),
                    stateAction: callbackData =>
                    {
                        creatorData.IsInvincible = false;
                        if (warningDamageVfx != null)
                        {
                            warningDamageVfx.InitDamageBox(OnTriggeredEntered);
                        }
                    },
                    endAction: _ =>
                    {
                        creatorData.ForceUpdatePathEvent?.Invoke();
                        _isJumping = false;
                        if (_warningVfx)
                            PoolManager.Instance.Return(_warningVfx);
                    }
                );
        }

        private void OnTriggeredEntered(IEntityData entity)
        {
            if (creatorData.EntityType.CanCauseDamage(entity.EntityType))
            {
                SimpleMessenger.Publish(MessageScope.EntityMessage,
                            new SentDamageMessage(EffectSource.FromSkill, EffectProperty.Normal, ownerModel.JumpDamageBonus, ownerModel.JumDamageFactors, creatorData, entity));
            }
        }

        protected override void CancelSkill()
        {
            base.CancelSkill();
            _isJumping = false;
            creatorData.IsInvincible = false;
            if (_warningVfx)
                PoolManager.Instance.Return(_warningVfx);
        }
    }
}
