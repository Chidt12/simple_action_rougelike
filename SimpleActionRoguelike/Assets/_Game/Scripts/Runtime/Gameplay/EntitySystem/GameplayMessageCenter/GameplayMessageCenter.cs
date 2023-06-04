using Runtime.Core.Message;
using Runtime.Message;
using System.Collections.Generic;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class GameplayMessageCenter
    {
        private List<ISubscription> _subscriptions;

        #region Class Methods

        public void Init()
        {
            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentHealMessage>(OnSentHeal));
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentDamageMessage>(OnSentDamage));
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentStatusEffectMessage>(OnSentStatusEffect));
        }

        public void Dispose()
        {
            foreach (var item in _subscriptions)
                item.Dispose();
        }

        private partial void OnSentStatusEffect(SentStatusEffectMessage message);

        private void OnSentHeal(SentHealMessage message)
        {

        }

        private partial void OnSentDamage(SentDamageMessage message);

        #endregion Class Methods
    }
}