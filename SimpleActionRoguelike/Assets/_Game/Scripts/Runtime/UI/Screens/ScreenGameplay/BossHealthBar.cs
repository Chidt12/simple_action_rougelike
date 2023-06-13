using Cysharp.Threading.Tasks;
using Runtime.Helper;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Gameplay.EntitySystem
{
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private Image _healthBarImage;
        [SerializeField] private Image _runHealthBarImage;
        [SerializeField] private Animator _animator;

        private CancellationTokenSource _cancellationTokenSource;
        private IEntitySkillData _bossSkillData;
        private IEntityStatData _bossStatData;

        public bool IsAvailable => _bossSkillData == null && _bossStatData == null;

        public void Init(IEntitySkillData bossSkillData, IEntityStatData bossStatData)
        {
            _animator.Play("default");
            _bossSkillData = bossSkillData;
            _bossStatData = bossStatData;
            _bossStatData.HealthStat.OnDamaged += OnDamaged;
            _bossStatData.HealthStat.OnHealed += OnHealed;
            _bossSkillData.OnTriggeredPhaseEvent += OnTriggeredPhase;

            _runHealthBarImage.fillAmount = _bossStatData.HealthStat.CurrentValue / _bossStatData.HealthStat.TotalValue;
            _healthBarImage.fillAmount = _bossStatData.HealthStat.CurrentValue / _bossStatData.HealthStat.TotalValue;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();

            if(_bossSkillData != null)
            {
                _bossSkillData.OnTriggeredPhaseEvent -= OnTriggeredPhase;
                _bossSkillData = null;
            }

            if (_bossStatData != null)
            {
                _bossStatData.HealthStat.OnDamaged -= OnDamaged;
                _bossStatData.HealthStat.OnHealed -= OnHealed;
                _bossStatData = null;
            }
        }

        private void OnTriggeredPhase()
        {
            _animator.Play("trigger_phase");
        }

        private void OnHealed(float arg1, EffectSource arg2, EffectProperty arg3)
        {
            _runHealthBarImage.fillAmount = _bossStatData.HealthStat.CurrentValue / _bossStatData.HealthStat.TotalValue;
            _healthBarImage.fillAmount = _bossStatData.HealthStat.CurrentValue / _bossStatData.HealthStat.TotalValue;
        }

        private void OnDamaged(float arg1, EffectSource arg2, EffectProperty arg3)
        {
            _animator.Play("get_hurt");
            _healthBarImage.fillAmount = _bossStatData.HealthStat.CurrentValue / _bossStatData.HealthStat.TotalValue;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new();
            StartHealthScrollAsync(_bossStatData.HealthStat.CurrentValue, _bossStatData.HealthStat.TotalValue, _cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid StartHealthScrollAsync(float currentHP, float maxHP, CancellationToken token)
        {
            var startRatio = _runHealthBarImage.fillAmount;
            var currentRatio = startRatio;
            var targetRatio = currentHP / maxHP;

            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: token);

            var animationTime = 0.5f;
            float currentTime = 0;
            while (currentTime <= animationTime)
            {
                currentTime += Time.deltaTime;
                _runHealthBarImage.fillAmount = currentRatio + (targetRatio - currentRatio) * (currentTime / animationTime);
                await UniTask.Yield(cancellationToken: token);
            }
            _runHealthBarImage.fillAmount = targetRatio;
        }
    }
}