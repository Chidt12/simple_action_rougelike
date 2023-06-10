using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Gameplay.TextDamage;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityDisplayTextDamageBehavior : EntityBehavior<IEntityStatData>, IDisposeEntityBehavior
    {
        [SerializeField] private Transform _topTransform;

        private const string TEXT_DAMAGE = "text_damage";
        private const string HERO_TEXT_DAMAGE = "hero_text_damage";
        private const string DODGE_TEXT = "dodge_text_damage";
        private const string CRIT_TEXT_DAMAGE = "crit_text_damage";
        private const string POISON_TEXT_DAMAGE = "poison_text_damage";
        private const string HEAL_TEXT = "heal_text";


        private IEntityData _ownerData;
        private CancellationTokenSource _cancellationTokenSource;

        protected override UniTask<bool> BuildDataAsync(IEntityStatData data)
        {
            if(data != null)
            {
                _ownerData = data;
                _cancellationTokenSource = new CancellationTokenSource();
                data.HealthStat.OnDamaged += OnDamaged;
                data.HealthStat.OnHealed += OnHealed;
                data.ReactionChangedEvent += OnReactionChanged;

                return UniTask.FromResult(true);
            }
            return UniTask.FromResult(false);
        }

        private void OnDamaged(float value, EffectSource effectSource, EffectProperty effectProperty)
        {
            if(value > 0)
                DisplayTextDamage(value, effectSource, effectProperty).Forget();
        }

        private async UniTask DisplayTextDamage(float value, EffectSource effectSource, EffectProperty effectProperty)
        {
            await TextDamageController.Instance.Spawn(_ownerData.EntityType.IsHero() ? HERO_TEXT_DAMAGE : TEXT_DAMAGE, value, false, _topTransform.position, _cancellationTokenSource.Token);
        }

        private void OnHealed(float value, EffectSource effectSource, EffectProperty effectProperty)
        {

        }

        private void OnReactionChanged(EntityReactionType reactionType)
        {
            if(reactionType == EntityReactionType.Dodge)
            {
                DisplayDodgeAsync().Forget();
            }
        }
        private async UniTaskVoid DisplayDodgeAsync()
        {
            var dodgeText = await PoolManager.Instance.Rent(DODGE_TEXT, token: _cancellationTokenSource.Token);
            dodgeText.transform.position = _topTransform.position;
        }
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
