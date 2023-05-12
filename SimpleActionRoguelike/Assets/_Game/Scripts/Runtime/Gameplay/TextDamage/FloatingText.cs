using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Runtime.Core.Pool;
using UnityEngine;

namespace Runtime.Gameplay.TextDamage
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 0.5f;
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _delayFade;
        [SerializeField] private float _fadeSpeed;

        private float _currentLifeTime;
        private float _currentFadeAmount;

        private void OnEnable()
        {
            _currentLifeTime = 0;
            _currentFadeAmount = 100;
            EnableOpacity();
        }

        private void Update()
        {
            if(_currentLifeTime < _lifeTime)
            {
                if(_currentLifeTime >= _delayFade)
                {
                    _currentFadeAmount -= Time.deltaTime * _fadeSpeed;
                    UpdateFade(_currentFadeAmount / 100f);
                }

                _currentLifeTime += Time.deltaTime;
                transform.position = new Vector2(transform.position.x, transform.position.y + _speed * Time.deltaTime);
            }
            else
            {
                OnComplete();
            }
        }

        protected virtual void EnableOpacity()
        {

        }

        protected virtual void UpdateFade(float value)
        {

        }

        protected virtual void OnComplete()
        {
            PoolManager.Instance.Return(gameObject);
        }
    }
}