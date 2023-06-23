using Runtime.Core.Message;
using Runtime.Message;
using System.Collections.Generic;
using System.Linq;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class GameplayMessageCenter
    {
        private List<IPreCalculateDamageModifier> _preCalculateDamageModifiers;
        private List<IPostCalculateDamageModifier> _postCalculateDamageModifiers;
        private List<IDamageModifier> _damageModifiers;
        private List<IFinalDamagedModifier> _finalDamageCreatedModifiers;
        private List<IHealModifier> _healModifiers;

        private List<ISubscription> _subscriptions;

        #region Class Methods

        public void Init()
        {
            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentHealMessage>(OnSentHeal));
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentDamageMessage>(OnSentDamage));
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentStatusEffectMessage>(OnSentStatusEffect));

            _preCalculateDamageModifiers = new();
            _postCalculateDamageModifiers = new();
            _damageModifiers = new();
            _finalDamageCreatedModifiers = new();
            _healModifiers = new();
        }

        public void Dispose()
        {
            foreach (var item in _subscriptions)
                item.Dispose();

            _preCalculateDamageModifiers.Clear();
            _postCalculateDamageModifiers.Clear();
            _damageModifiers.Clear();
            _finalDamageCreatedModifiers.Clear();
        }

        public void AddDamageModifier(IDamageModifier damageModifier)
        {
            _damageModifiers.Add(damageModifier);
            _damageModifiers = _damageModifiers.OrderBy(x => x.Priority).ToList();
        }

        public void RemoveDamageModifier(IDamageModifier damageModifier)
        {
            _damageModifiers.Remove(damageModifier);
            _damageModifiers = _damageModifiers.OrderBy(x => x.Priority).ToList();
        }

        public void AddHealModifier(IHealModifier healModifier)
        {
            _healModifiers.Add(healModifier);
            _healModifiers = _healModifiers.OrderBy(x => x.Priority).ToList();
        }

        public void RemoveHealModifier(IHealModifier healModifier)
        {
            _healModifiers.Remove(healModifier);
            _healModifiers = _healModifiers.OrderBy(x => x.Priority).ToList();
        }

        public void AddPreCalculateDamageModifier(IPreCalculateDamageModifier preCalculateDamageModifier)
        {
            _preCalculateDamageModifiers.Add(preCalculateDamageModifier);
            _preCalculateDamageModifiers = _preCalculateDamageModifiers.OrderBy(x => x.Priority).ToList();
        }

        public void RemovePreCalculateDamageModifier(IPreCalculateDamageModifier preCalculateDamageModifier)
        {
            _preCalculateDamageModifiers.Remove(preCalculateDamageModifier);
            _preCalculateDamageModifiers = _preCalculateDamageModifiers.OrderBy(x => x.Priority).ToList();
        }

        public void AddPostCalculateDamageModifier(IPostCalculateDamageModifier postCalculateDamageModifier)
        {
            _postCalculateDamageModifiers.Add(postCalculateDamageModifier);
            _postCalculateDamageModifiers = _postCalculateDamageModifiers.OrderBy(x => x.Priority).ToList();
        }

        public void RemovePostCalculateDamageModifier(IPostCalculateDamageModifier postCalculateDamageModifier)
        {
            _postCalculateDamageModifiers.Remove(postCalculateDamageModifier);
            _postCalculateDamageModifiers = _postCalculateDamageModifiers.OrderBy(x => x.Priority).ToList();
        }


        public void AddFinalDamageCreatedModifier(IFinalDamagedModifier finalDamagedModifier)
        {
            _finalDamageCreatedModifiers.Add(finalDamagedModifier);
            _finalDamageCreatedModifiers = _finalDamageCreatedModifiers.OrderBy(x => x.Priority).ToList();
        }

        public void RemoveFinalDamageCreatedModifier(IFinalDamagedModifier finalDamagedModifier)
        {
            _finalDamageCreatedModifiers.Remove(finalDamagedModifier);
            _finalDamageCreatedModifiers = _finalDamageCreatedModifiers.OrderBy(x => x.Priority).ToList();
        }

        private partial void OnSentStatusEffect(SentStatusEffectMessage message);

        private void OnSentHeal(SentHealMessage message) { }

        private partial void OnSentDamage(SentDamageMessage message);

        #endregion Class Methods
    }
}