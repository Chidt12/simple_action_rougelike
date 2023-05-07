using Runtime.Core.Message;
using Runtime.Core.Singleton;
using Runtime.Message;
using System.Collections.Generic;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class MessageCenter : MonoSingleton<MessageCenter>
    {
        private List<ISubscription> _subscriptions;

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentHealMessage>(OnSentHeal));
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentDamageMessage>(OnSentDamage));
            _subscriptions.Add(SimpleMessenger.Scope(MessageScope.EntityMessage).Subscribe<SentStatusEffectMessage>(OnSentStatusEffect));
        }

        private void OnDestroy()
        {
            foreach (var item in _subscriptions)
                item.Dispose();
        }

        #endregion API Methods

        #region Class Methods

        private partial void OnSentStatusEffect(SentStatusEffectMessage message);

        private void OnSentHeal(SentHealMessage message)
        {

        }

        private partial void OnSentDamage(SentDamageMessage message);

        #endregion Class Methods
    }
}