using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using Runtime.Definition;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityUseSkillBehavior : EntityBehavior<IEntityControlData, IEntitySkillData, IEntityStatData>, IDisposeEntityBehavior, IEntityControlCastRangeProxy
    {
        private static string s_warningSkillVFX = "warning_execute_skill_vfx";

        [SerializeField]
        private Transform _displayWarningTransform;

        private IEntityControlData _controlData;
        private IEntitySkillData _skillData;
        private ISkillStrategy[] _skillStrategies;

        private int _currentlyUsedSkillIndex;
        private List<SkillModel> _skillModels;
        private CancellationTokenSource[] _skillCooldownCancellationTokenSources;
        private CancellationTokenSource _cancellationTokenSource;

        public float CastRange => _skillModels[_currentlyUsedSkillIndex].CastRange;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data, IEntitySkillData skillData, IEntityStatData statData)
        {
            _controlData = data;
            _skillData = skillData;
            if(skillData != null && data != null && statData != null)
            {
                _skillModels = skillData.SkillModels;
                if (_skillModels != null && _skillModels.Count > 0)
                {
                    var cooldownReduction = statData.GetTotalStatValue(StatType.CooldownReduction);
                    _skillCooldownCancellationTokenSources = new CancellationTokenSource[_skillModels.Count];
                    _skillStrategies = new ISkillStrategy[_skillModels.Count];

                    for (int i = 0; i < _skillModels.Count; i++)
                    {
                        var skillModel = _skillModels[i];
                        skillModel.Cooldown = skillModel.Cooldown * (1 - cooldownReduction);
                        var skillStrategy = SkillStrategyFactory.GetSkillStrategy(skillModel.SkillType);
                        skillStrategy.Init(skillModel, data);
                        skillStrategy.SetTriggerEventProxy(GetComponent<IEntityTriggerActionEventProxy>());
                        _skillStrategies[i] = skillStrategy;
                    }

                    _controlData.PlayActionEvent += OnActionTriggered;
                    _currentlyUsedSkillIndex = 0;
                    _cancellationTokenSource = new CancellationTokenSource();
                    FinishSkill();
                    return UniTask.FromResult(true);
                }
            }
            
            return UniTask.FromResult(false);
        }

        private void OnActionTriggered(ActionInputType actionInputType)
        {
            if (actionInputType == ActionInputType.UseSkill1 ||
                actionInputType == ActionInputType.UseSkill2 ||
                actionInputType == ActionInputType.UseSkill3)
            {
                var skillIndex = actionInputType.GetSkillIndex();
                if (skillIndex >= _skillModels.Count)
                    skillIndex = 0;

                var skillModel = _skillModels[skillIndex];
                if (skillModel != null && skillModel.IsReady)
                {
                    if (_skillData.CheckCanUseSkill())
                    {
                        _skillData.IsPlayingSkill = true;
                        _currentlyUsedSkillIndex = skillIndex;
                        _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustPlaySkill);
                        StartExecutingSkillAsync(_skillStrategies[skillIndex], skillIndex).Forget();
                    }
                }
            }
        }

        private async UniTaskVoid StartExecutingSkillAsync(ISkillStrategy skillStrategy, int skillIndex)
        {
            await skillStrategy.ExecuteAsync(_cancellationTokenSource.Token, skillIndex, FinishedPrecheck);
            FinishSkill();
        }

        private async UniTask FinishedPrecheck()
        {
            _controlData.SetMoveDirection(Vector2.zero);
            if (_controlData.EntityType.IsDisplayWarningExecuteSkill())
            {
                var warningVFX = await PoolManager.Instance.Rent(s_warningSkillVFX, token: _cancellationTokenSource.Token);
                warningVFX.transform.SetParent(_displayWarningTransform);
                warningVFX.transform.localPosition = Vector2.zero;
            }
        }

        private void FinishSkill()
        {
            if (!_skillModels[_currentlyUsedSkillIndex].CurrentSkillPhase.IsCast())
            {
                if(_skillModels[_currentlyUsedSkillIndex].CurrentSkillPhase == SkillPhase.Precheck)
                {
                    _skillData.IsPlayingSkill = false;
                    _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustFinishedUseSkill);

                    _skillModels[_currentlyUsedSkillIndex].CurrentSkillPhase = SkillPhase.Ready;
                }
            }
            else
            {
                _skillData.IsPlayingSkill = false;
                _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustFinishedUseSkill);

                _skillModels[_currentlyUsedSkillIndex].CurrentSkillPhase = SkillPhase.Cooldown;
                var cancellationTokenSource = new CancellationTokenSource();
                RunCountdownSkillAsync(_skillModels[_currentlyUsedSkillIndex], cancellationTokenSource.Token).Forget();
                _skillCooldownCancellationTokenSources[_currentlyUsedSkillIndex] = cancellationTokenSource;
            }
        }

        private async UniTaskVoid RunCountdownSkillAsync(SkillModel skillModel, CancellationToken cancellationToken)
        {
            skillModel.CurrentCooldown = skillModel.Cooldown;
            while(skillModel.CurrentCooldown > 0)
            {
                await UniTask.Yield(cancellationToken);
                skillModel.CurrentCooldown -= Time.deltaTime;
            }
            skillModel.CurrentSkillPhase = SkillPhase.Ready;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            foreach (var skillStrategy in _skillStrategies)
                skillStrategy.Dispose();

            foreach (var skillCooldownCancellationTokenSource in _skillCooldownCancellationTokenSources)
                skillCooldownCancellationTokenSource?.Cancel();
        }
    }
}
