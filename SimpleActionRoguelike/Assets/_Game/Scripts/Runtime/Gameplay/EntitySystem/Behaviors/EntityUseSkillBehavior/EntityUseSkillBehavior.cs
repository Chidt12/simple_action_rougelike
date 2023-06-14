using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using Runtime.Definition;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityUseSkillBehavior : EntityBehavior<IEntityControlData, IEntitySkillData, IEntityStatData, IEntityStatusData>, 
        IDisposeEntityBehavior, IEntityControlCastRangeProxy
    {
        [SerializeField] private bool _hideWarning;
        [HideIf(nameof(_hideWarning))]
        [SerializeField] private string _warningSkillVFX = "warning_execute_skill_vfx";
        [HideIf(nameof(_hideWarning))]
        [SerializeField] private Transform _warningDisplayPosition;
        [HideIf(nameof(_hideWarning))]
        [SerializeField] private float _timeDisplayWarning = 0.3f;
        [SerializeField] private float _delayBeforeExecuteSkillTime = 3f;
        private bool _finishedDelay;

        private IEntityControlData _controlData;
        private IEntityStatusData _statusData;
        private IEntitySkillData _skillData;
        private ISkillStrategy[] _skillStrategies;

        private int _currentlyUsedSkillIndex;
        private List<SkillModel> _skillModels;
        private CancellationTokenSource[] _skillCooldownCancellationTokenSources;
        private CancellationTokenSource _cancellationTokenSource;
        private GameObject _warningGameObject;

        public float CastRange => _skillModels[_currentlyUsedSkillIndex].CastRange;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data, IEntitySkillData skillData, IEntityStatData statData, IEntityStatusData statusData)
        {
            _finishedDelay = false;
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

                    if (statusData != null)
                    {
                        _statusData = statusData;
                        _statusData.UpdateCurrentStatus += OnUpdateCurrentStatus;
                    }

                    if (_delayBeforeExecuteSkillTime > 0)
                    {
                        StartCountimeDelayAsync().Forget();
                    }
                    else _finishedDelay = true;

                    return UniTask.FromResult(true);
                }
            }

            return UniTask.FromResult(false);
        }

        private async UniTaskVoid StartCountimeDelayAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeExecuteSkillTime), cancellationToken: _cancellationTokenSource.Token);
            _finishedDelay = true;
        }

        private void OnUpdateCurrentStatus()
        {
            if (_statusData.CurrentState.IsInHardCCStatus())
            {
                var triggerResult = _skillStrategies[_currentlyUsedSkillIndex].Cancel();
                if (triggerResult.Result)
                {
                    FinishSkill();
                }
            }
        }

        private void OnActionTriggered(ActionInputType actionInputType)
        {
            if (!_finishedDelay)
                return;

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
                        StartExecutingSkillAsync(_skillStrategies[skillIndex], _skillModels[skillIndex].SkillIndex).Forget();
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
            if (!_hideWarning)
            {
                _warningGameObject = await PoolManager.Instance.Rent(_warningSkillVFX, token: _cancellationTokenSource.Token);
                _warningGameObject.transform.SetParent(_warningDisplayPosition);
                _warningGameObject.transform.localPosition = Vector2.zero;
                await UniTask.Delay(TimeSpan.FromSeconds(_timeDisplayWarning), cancellationToken: _cancellationTokenSource.Token);
                PoolManager.Instance.Return(_warningGameObject);
                _warningGameObject = null;
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
            if (_warningGameObject)
                PoolManager.Instance.Return(_warningGameObject);

            _cancellationTokenSource?.Cancel();
            foreach (var skillStrategy in _skillStrategies)
                skillStrategy.Dispose();

            foreach (var skillCooldownCancellationTokenSource in _skillCooldownCancellationTokenSources)
                skillCooldownCancellationTokenSource?.Cancel();
        }
    }
}
