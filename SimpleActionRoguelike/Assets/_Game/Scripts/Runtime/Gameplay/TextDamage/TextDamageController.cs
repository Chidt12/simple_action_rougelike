using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using Runtime.Core.Singleton;
using Runtime.Helper;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.TextDamage
{
    public class TextDamageController : MonoSingleton<TextDamageController>
    {
        [SerializeField]
        private int _maxDamageTextNumber;
        private List<TextDamage> _damageFloatingTexts;

        protected override void Awake()
        {
            base.Awake();
            _damageFloatingTexts = new();
        }

        public async UniTask Spawn(string assetName, float value, Vector2 spawnPosition, CancellationToken token)
        {
            while (_damageFloatingTexts.Count >= _maxDamageTextNumber)
            {
                // Despawn oldest text;
                var firstDamageFloatingText = _damageFloatingTexts[0];
                _damageFloatingTexts.Remove(firstDamageFloatingText);
                PoolManager.Instance.Return(firstDamageFloatingText.gameObject);
            }

            var damageTextObject = await PoolManager.Instance.Rent(assetName, token: token);
            var damageText = damageTextObject.GetOrAddComponent<TextDamage>();
            damageText.Init(value, spawnPosition);
            _damageFloatingTexts.Add(damageText);
        }

        public void Despawn(TextDamage floatingText)
        {
            _damageFloatingTexts.Remove(floatingText);
            PoolManager.Instance.Return(floatingText.gameObject);
        }
    }
}
