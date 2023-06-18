using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Message;
using Runtime.Helper;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class JumpAheadSkillStrategy : SkillStrategy<JumpAheadSkillModel>
    {
        private DamageBox _creatorDamageBox;
        private bool _isJumping;
        private const string DAMAGE_BOX_NAME = "hit_attack_damage_box";

        protected override void Init(JumpAheadSkillModel skillModel)
        {
            base.Init(skillModel);

            var damageBoxGameObject = creatorData.EntityTransform.FindChildTransform(DAMAGE_BOX_NAME);
            if (damageBoxGameObject)
            {
                _creatorDamageBox = damageBoxGameObject.GetComponent<DamageBox>();
                _creatorDamageBox.gameObject.SetActive(false);
            }
        }

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

                        return true;
                    }
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
                            JumpAhead(direction, cancellationToken).Forget();
                        }
                    );

                await UniTask.WaitUntil(() => !_isJumping, cancellationToken: cancellationToken);

                if(i != ownerModel.NumberOfJump - 1)
                    await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.DelayBetweenJump), cancellationToken: cancellationToken);
            }
        }

        private async UniTaskVoid JumpAhead(Vector2 jumpDirection, CancellationToken token)
        {
            if (_creatorDamageBox)
            {
                _creatorDamageBox.gameObject.SetActive(true);
                _creatorDamageBox.Init(OnTriggeredEntered);
            }

            var predictJumpPosition = creatorData.Position;
            if (ownerModel.DependTarget && creatorData.Target != null)
            {
                predictJumpPosition = creatorData.Target.Position;
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
            }

            var originPosition = creatorData.Position;
            var middlePosition = new Vector2((predictJumpPosition.x + originPosition.x) / 2, (predictJumpPosition.y + originPosition.y)/2 + ownerModel.JumpHeight);

            float currentTime = 0;

            while (currentTime < ownerModel.JumpDuration)
            {
                currentTime += Time.deltaTime;
                var moveToPosition = Helper.Helper.Bezier(originPosition, middlePosition, predictJumpPosition, Mathf.Clamp01(currentTime / ownerModel.JumpDuration));

                if (MapManager.Instance.IsWalkable(moveToPosition))
                    creatorData.ForceUpdatePosition?.Invoke(moveToPosition);
                else
                    break;

                await UniTask.Yield(token);
            }

            creatorData.ForceUpdatePathEvent?.Invoke();
            _isJumping = false;
            if (_creatorDamageBox)
                _creatorDamageBox.gameObject.SetActive(false);
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
            if (_creatorDamageBox)
                _creatorDamageBox.gameObject.SetActive(false);
        }
    }
}