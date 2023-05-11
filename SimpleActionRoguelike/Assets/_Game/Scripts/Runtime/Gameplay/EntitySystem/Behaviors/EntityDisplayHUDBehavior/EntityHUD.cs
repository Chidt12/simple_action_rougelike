using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityHUD : MonoBehaviour
    {
        [SerializeField]
        private Transform _healthBarSliderAnchor;
        [SerializeField]
        private Transform _shieldBarSliderAnchor;
        [SerializeField]
        private GameObject _shieldContainer;

        public void Init(float currentDefense, float maxDefense)
        {
            _shieldBarSliderAnchor.transform.localScale = new Vector2(1, 1);
            _healthBarSliderAnchor.transform.localScale = new Vector2(1,1);

            UpdateShieldBar(currentDefense, maxDefense);
            SetVisibility(true);
        }

        public void UpdateShieldBar(float currentDefense, float maxDefense)
        {
            if (maxDefense <= 0 || currentDefense <= 0)
            {
                _shieldContainer.SetActive(false);
                return;
            }

            _shieldContainer.SetActive(true);
            _shieldBarSliderAnchor.transform.localScale = new Vector2(currentDefense / maxDefense, 1);
        }

        public void UpdateHealthBar(float currentHP, float maxHP)
        {
            _healthBarSliderAnchor.transform.localScale = new Vector2(currentHP / maxHP, 1);
        }

        public void SetVisibility(bool isVisible)
            => gameObject.SetActive(isVisible);
    }
}
