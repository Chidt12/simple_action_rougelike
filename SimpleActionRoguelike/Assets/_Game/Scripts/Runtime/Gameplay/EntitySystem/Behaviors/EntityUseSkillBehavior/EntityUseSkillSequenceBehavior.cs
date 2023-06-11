using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using Runtime.Definition;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityUseSkillSequenceBehavior : EntityBehavior<IEntityControlData, IEntitySkillData, IEntityStatData, IEntityStatusData>, IDisposeEntityBehavior, IEntityControlCastRangeProxy
    {
        [SerializeField] private bool _hideWarning;
        [HideIf(nameof(_hideWarning))]
        [SerializeField] private string _warningSkillVFX = "warning_execute_skill_vfx";
        [HideIf(nameof(_hideWarning))]
        [SerializeField] private Transform _warningDisplayPosition;
        [HideIf(nameof(_hideWarning))]
        [SerializeField] private float _timeDisplayWarning = 0.3f;
        [SerializeField] private float _delayBeforeExecuteSkillTime = 3f;

        private IEntityControlData _controlData;
        private IEntityStatusData _statusData;
        private IEntitySkillData _skillData;
        private ISkillStrategy[] _skillStrategies;

        private bool _finishedDelay;
        private List<SkillModel> _skillModels;
        private int _currentlyUsedSkillIndex;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _delayCancellationTokenSource;
        private GameObject _warningGameObject;

        public float CastRange => _skillModels[_currentlyUsedSkillIndex].CastRange;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data, IEntitySkillData skillData, IEntityStatData statData, IEntityStatusData statusData)
        {
            _finishedDelay = false;

            if (skillData != null && data != null && statData != null)
            {
                _skillModels = skillData.SkillModels;
                if (_skillModels != null && _skillModels.Count > 0)
                {

                    
                    
                }

                _controlData = data;
                _skillData = skillData;
                _controlData.PlayActionEvent += OnActionTriggered;
                _statusData = statusData;
                _statusData.UpdateCurrentStatus += OnUpdateCurrentStatus;

                _cancellationTokenSource = new CancellationTokenSource();

                return UniTask.FromResult(true);
            }

            return UniTask.FromResult(false);
        }

        private void SetUpSkillModels()
        {
            _skillStrategies = new ISkillStrategy[_skillModels.Count];

            for (int i = 0; i < _skillModels.Count; i++)
            {
                var skillModel = _skillModels[i];
                var skillStrategy = SkillStrategyFactory.GetSkillStrategy(skillModel.SkillType);
                skillStrategy.Init(skillModel, _controlData);
                skillStrategy.SetTriggerEventProxy(GetComponent<IEntityTriggerActionEventProxy>());
                _skillStrategies[i] = skillStrategy;
            }

            _currentlyUsedSkillIndex = 0;
            FinishSkill(true);
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
                    FinishSkill(false);
            }
        }

        private void OnActionTriggered(ActionInputType actionInputType)
        {
            if (!_finishedDelay)
                return;

            if (actionInputType == ActionInputType.UseSkill1)
            {
                var skillModel = _skillModels[_currentlyUsedSkillIndex];
                if (skillModel != null && skillModel.IsReady)
                {
                    if (_skillData.CheckCanUseSkill())
                    {
                        _skillData.IsPlayingSkill = true;
                        _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustPlaySkill);
                        StartExecutingSkillAsync(_skillStrategies[_currentlyUsedSkillIndex], _currentlyUsedSkillIndex).Forget();
                    }
                }
            }
        }

        private async UniTaskVoid StartExecutingSkillAsync(ISkillStrategy skillStrategy, int skillIndex)
        {
            await skillStrategy.ExecuteAsync(_cancellationTokenSource.Token, skillIndex, FinishedPrecheck);
            FinishSkill(false);
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

        private void FinishSkill(bool init)
        {
            if (!_skillModels[_currentlyUsedSkillIndex].CurrentSkillPhase.IsCast())
            {
                if (_skillModels[_currentlyUsedSkillIndex].CurrentSkillPhase == SkillPhase.Precheck)
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
                _skillModels[_currentlyUsedSkillIndex].CurrentSkillPhase = SkillPhase.Ready;
                _delayCancellationTokenSource?.Cancel();
                _delayCancellationTokenSource = new();

                if (init)
                {
                    RunDelayExecuteSkillAsync(_delayCancellationTokenSource.Token).Forget();
                }
                else
                {
                    if (_delayBeforeExecuteSkillTime > 0)
                    {
                        StartCountimeDelayAsync().Forget();
                    }
                    else _finishedDelay = true;
                }
            }
        }

        private async UniTaskVoid RunDelayExecuteSkillAsync(CancellationToken cancellationToken)
        {
            _finishedDelay = false;

            _finishedDelay = true;
        }

        public void Dispose()
        {
            if (_warningGameObject)
                PoolManager.Instance.Return(_warningGameObject);

            _cancellationTokenSource?.Cancel();
            _delayCancellationTokenSource?.Cancel();
            foreach (var skillStrategy in _skillStrategies)
                skillStrategy.Dispose();
        }
    }
}