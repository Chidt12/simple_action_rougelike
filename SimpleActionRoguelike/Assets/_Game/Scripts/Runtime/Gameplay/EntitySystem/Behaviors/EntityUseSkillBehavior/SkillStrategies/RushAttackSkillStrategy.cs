using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Helper;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class RushAttackSkillStrategy : SkillStrategy<RushAttackSkillModel>
    {
        private static readonly float s_rushDurationOffset = 0.2f;
        private DamageBox _creatorDamageBox;
        private GameObject _indicatorGameObject;
        private const string INDICATOR = "rush_indicator";
        private const string DAMAGE_BOX_NAME = "rush_attack_damage_box";
        private bool _isHittedTarget;

        protected override void Init(RushAttackSkillModel skillModel)
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
                    if (distance <= ownerModel.RushRange)
                    {
                        var direction = creatorData.Target.Position - creatorData.Position;
                        var rayCastChecks = Physics2D.RaycastAll(creatorData.Position, direction, distance);

                        foreach (var rayCastCheck in rayCastChecks)
                        {
                            if (rayCastCheck.collider.gameObject.layer == Layers.OBJECT_LAYER)
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
            _isHittedTarget = false;
            entityTriggerActionEventProxy.TriggerEvent(index.GetUseSkillByIndex());

            creatorData.IsPausedControl = true;
            creatorData.SetMoveDirection(Vector2.zero);

            var stopRushing = false;
            for (int i = 0; i < ownerModel.NumberOfRushTime; i++)
            {
                if (_creatorDamageBox)
                {
                    _creatorDamageBox.gameObject.SetActive(true);
                    _creatorDamageBox.StartDamage(creatorData, EffectSource.FromSkill, EffectProperty.Normal, ownerModel.RushDamageBonus, ownerModel.RushDamageFactors, default);
                }

                var originPosition = creatorData.Position;
                var direction = (creatorData.Target.Position - creatorData.Position).normalized;
                creatorData.SetFaceDirection(direction);

                var currentDuration = 0.0f;
                _indicatorGameObject = await PoolManager.Instance.Rent(INDICATOR, token: cancellationToken);
                var indicator = _indicatorGameObject.GetOrAddComponent<RushAttackIndicator>();
                indicator.Init(ownerModel.RushWidth, ownerModel.RushRange);
                indicator.transform.position = _creatorDamageBox.CenterBoxPosition;
                indicator.transform.rotation = direction.ToQuaternion();
                await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.WarningRushDuration), cancellationToken: cancellationToken);
                PoolManager.Instance.Return(_indicatorGameObject);

                while (currentDuration <= ownerModel.RushDuration)
                {
                    if (creatorData.IsDead)
                    {
                        stopRushing = true;
                        break;
                    }

                    currentDuration += Time.deltaTime;
                    var easeValue = Easing.Linear(0.0f, 1.0f, Mathf.Clamp01(currentDuration / ownerModel.RushDuration));
                    float interpolationValue = Mathf.Lerp(0, ownerModel.RushRange, easeValue);
                    Vector2 dashPosition = originPosition + direction * interpolationValue;

                    if (MapManager.Instance.IsWalkable(dashPosition))
                    {
                        creatorData.ForceUpdatePosition.Invoke(dashPosition, false);
                        if(ownerModel.StopRushingAfterHitTarget && _isHittedTarget)
                        {
                            _creatorDamageBox.gameObject.SetActive(false);
                            stopRushing = true;
                            break;
                        }
                    }
                    else
                    {
                        _creatorDamageBox.gameObject.SetActive(false);
                        await UniTask.Delay(TimeSpan.FromSeconds(s_rushDurationOffset), cancellationToken: cancellationToken);
                        break;
                    }

                    await UniTask.Yield(cancellationToken: cancellationToken);
                }

                if (stopRushing)
                    break;

                _creatorDamageBox.gameObject.SetActive(false);

                if (i != ownerModel.NumberOfRushTime - 1)
                    await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.DelayBetweenRush), cancellationToken: cancellationToken);
            }

            _creatorDamageBox.gameObject.SetActive(false);
            creatorData.IsPausedControl = false;;
        }

        protected override void CancelSkill()
        {
            creatorData.IsPausedControl = false;
            if (_indicatorGameObject)
            {
                PoolManager.Instance.Return(_indicatorGameObject);
            }
            _creatorDamageBox.gameObject.SetActive(false);
            base.CancelSkill();
        }
    }
}