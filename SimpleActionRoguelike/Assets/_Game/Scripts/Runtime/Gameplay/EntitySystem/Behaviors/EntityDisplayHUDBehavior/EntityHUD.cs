using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityHUD : MonoBehaviour
    {
        [SerializeField] private Transform _healthBarRunAnchor;
        [SerializeField] private Transform _healthBarSliderAnchor;
        [SerializeField] private SpriteRenderer _healthBarContainer;

        private CancellationTokenSource _cancellationTokenSource;

        public void Init(float currentHp, float maxHp)
        {
            _healthBarSliderAnchor.localScale = new Vector2(1,1);
            UpdateHealthBar(currentHp, maxHp, false);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void UpdateHealthBar(float currentHP, float maxHP, bool isHurt)
        {
            if(currentHP >= maxHP)
            {
                _healthBarContainer.gameObject.SetActive(false);
                return;
            }

            if (isHurt)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new();
                StartHealthScrollAsync(currentHP, maxHP, _cancellationTokenSource.Token).Forget();
            }

            _healthBarContainer.gameObject.SetActive(true);
            _healthBarSliderAnchor.localScale = new Vector2(currentHP / maxHP, 1);
        }

        private async UniTaskVoid StartHealthScrollAsync(float currentHP, float maxHP, CancellationToken token)
        {
            var currentRatio = _healthBarSliderAnchor.localScale.x;
            var targetRatio = currentHP / maxHP;

            _healthBarRunAnchor.localScale = new Vector2(currentRatio, 1);
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: token);

            while(currentRatio < targetRatio)
            {
                currentRatio = currentRatio + 0.02f;
                _healthBarRunAnchor.localScale = new Vector2(currentRatio, 1);
                await UniTask.Yield(cancellationToken: token);
            }

            _healthBarRunAnchor.localScale = new Vector2(targetRatio, 1);
        }
    }
}
