using Runtime.Core.Message;
using Runtime.Message;
using System.Collections.Generic;
using System.Linq;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class GameplayMessageCenter
    {
        private List<IPreCalculateDamageModifier> preCalculateDamageModifiers;
        private List<IPostCalculateDamageModifier> postCalculateDamageModifiers;
        private List<IDamageModifier> damageModifiers;
        private List<IFinalDamagedModifier> finalDamageCreatedModifiers;

        private List<ISubscription> _subscriptions;

        #region Class Methods

        public void Init()
        {
            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentHealMessage>(OnSentHeal));
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentDamageMessage>(OnSentDamage));
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentStatusEffectMessage>(OnSentStatusEffect));

            preCalculateDamageModifiers = new();
            postCalculateDamageModifiers = new();
            damageModifiers = new();
            finalDamageCreatedModifiers = new();
        }

        public void Dispose()
        {
            foreach (var item in _subscriptions)
                item.Dispose();

            preCalculateDamageModifiers.Clear();
            postCalculateDamageModifiers.Clear();
            damageModifiers.Clear();
            finalDamageCreatedModifiers.Clear();
        }

        public void AddDamageModifier(IDamageModifier damageModifier)
        {
            damageModifiers.Add(damageModifier);
            damageModifiers = damageModifiers.OrderBy(x => x.Priority).ToList();
        }

        public void AddPreCalculateDamageModifier(IPreCalculateDamageModifier preCalculateDamageModifier)
        {
            preCalculateDamageModifiers.Add(preCalculateDamageModifier);
        }

        public void AddPostCalculateDamageModifier(IPostCalculateDamageModifier postCalculateDamageModifier)
        {
            postCalculateDamageModifiers.Add(postCalculateDamageModifier);
        }

        public void AddFinalDamageCreatedModifier(IFinalDamagedModifier finalDamagedModifier)
        {
            finalDamageCreatedModifiers.Add(finalDamagedModifier);
        }

        public void RemoveFinalDamageCreatedModifier(IFinalDamagedModifier finalDamagedModifier)
        {
            finalDamageCreatedModifiers.Remove(finalDamagedModifier);
        }

        private partial void OnSentStatusEffect(SentStatusEffectMessage message);

        private void OnSentHeal(SentHealMessage message) { }

        private partial void OnSentDamage(SentDamageMessage message);

        #endregion Class Methods
    }
}