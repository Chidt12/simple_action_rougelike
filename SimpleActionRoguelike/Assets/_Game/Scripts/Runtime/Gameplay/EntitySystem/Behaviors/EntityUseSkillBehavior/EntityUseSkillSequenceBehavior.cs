using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Definition;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ZBase.Foundation.PubSub;
using Runtime.Message;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityUseSkillSequenceBehavior : EntityBehavior<IEntityControlData, IEntitySkillData, IEntityStatData, IEntityStatusData, IEntityAutoInputData>, IDisposeEntityBehavior, IEntityControlCastRangeProxy
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
        private IEntityAutoInputData _autoInputData;
        private IEntityStatData _statData;
        private ISkillStrategy[] _skillStrategies;

        private List<SkillModel> _skillModels;
        private List<float> _skillDelayTimes;
        private List<AutoInputStrategyType> _autoInputStrategyTypes;

        private bool _finishedDelay;
        private int _currentlyUsedSkillIndex;
        private CancellationTokenSource _cancellationTokenSource;
        private GameObject _warningGameObject;

        private TriggerPhase _currentTriggerPhase;
        private bool _isFinalTriggerPhase;
        private ISubscription _subscription;

        public float CastRange => _skillModels[_currentlyUsedSkillIndex].CastRange;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data, IEntitySkillData skillData, IEntityStatData statData, IEntityStatusData statusData, IEntityAutoInputData autoInputData)
        {
            _finishedDelay = false;

            if (skillData != null && data != null && statData != null && autoInputData != null)
            {
                _autoInputData = autoInputData;
                _skillData = skillData;
                _controlData = data;
                _controlData.PlayActionEvent += OnActionTriggered;
                _statusData = statusData;
                _statusData.UpdateCurrentStatus += OnUpdateCurrentStatus;
                _statData = statData;
                _statData.HealthStat.OnDamaged += OnDamaged;
                _subscription = SimpleMessenger.Subscribe<EntityDiedMessage>(OnEntityDied);

                (_isFinalTriggerPhase, _currentTriggerPhase) = _skillData.GetNextTriggerPhase(new TriggerPhase());
                SetUpSkillModels();

                return UniTask.FromResult(true);
            }

            return UniTask.FromResult(false);
        }

        private void OnDamaged(float arg1, EffectSource arg2, EffectProperty arg3)
        {
            if (!_isFinalTriggerPhase)
            {
                if(_currentTriggerPhase.triggerPhaseType == TriggerPhaseType.HealthDecrease)
                {
                    var currentHealth = _statData.HealthStat.CurrentValue / _statData.HealthStat.TotalValue;
                    if(currentHealth <= _currentTriggerPhase.triggerPhaseHealth)
                        ChangePhase();
                }
            }
        }

        private void OnEntityDied(EntityDiedMessage message)
        {
            if (!_isFinalTriggerPhase)
            {
                if (_currentTriggerPhase.triggerPhaseType == TriggerPhaseType.EntityKilled)
                {
                    if (message.EntityData.EntityId == _currentTriggerPhase.triggerPhaseEntityId)
                        ChangePhase();
                }
            }
        }

        private void ChangePhase()
        {
            (_isFinalTriggerPhase, _currentTriggerPhase) = _skillData.GetNextTriggerPhase(new TriggerPhase());
            ResetData();
            SetUpSkillModels();
        }

        private void SetUpSkillModels()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var indexes = _skillData.GetSequenceSkillModelIndexes(_currentTriggerPhase);
            _skillModels = new();
            _skillDelayTimes = new();
            _autoInputStrategyTypes = new();

            foreach (var index in indexes)
            {
                _skillModels.Add(_skillData.SkillModels[index]);
                _skillDelayTimes.Add(_skillData.SkillDelayTimes[index]);
                _autoInputStrategyTypes.Add(_autoInputData.AutoInputStrategyTypes[index]);
            }

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
            _autoInputData.SetCurrentAutoInputStrategy(_autoInputStrategyTypes[_currentlyUsedSkillIndex]);
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

                if (!init)
                {
                    var delayTime = _skillDelayTimes[_currentlyUsedSkillIndex];
                    if(_currentlyUsedSkillIndex >= _skillModels.Count - 1)
                        _currentlyUsedSkillIndex = 0;
                    else
                        _currentlyUsedSkillIndex++;

                    var autoInputStrategyType = _autoInputStrategyTypes[_currentlyUsedSkillIndex];
                    _autoInputData.SetCurrentAutoInputStrategy(autoInputStrategyType);
                    RunDelayExecuteSkillAsync(delayTime, _cancellationTokenSource.Token).Forget();
                }
                else
                {
                    if (_delayBeforeExecuteSkillTime > 0)
                        StartCountimeDelayAsync().Forget();
                    else _finishedDelay = true;
                }
            }
        }

        private async UniTaskVoid RunDelayExecuteSkillAsync(float delayTime, CancellationToken cancellationToken)
        {
            _finishedDelay = false;
            await UniTask.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken: cancellationToken);
            _finishedDelay = true;
        }

        public void Dispose()
        {
            ResetData();
            _subscription?.Dispose();
        }

        private void ResetData()
        {
            if (_warningGameObject)
                PoolManager.Instance.Return(_warningGameObject);

            _cancellationTokenSource?.Cancel();
            if(_skillStrategies != null)
                foreach (var skillStrategy in _skillStrategies)
                    skillStrategy.Dispose();
        }
    }
}